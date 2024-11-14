using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ServiceAcademy.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public AssessmentController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost]
        public IActionResult Create(QuizModel quizModel, int NumberOfQuestions, int ProgramId)
        {
            // Check if ProgramId exists
            var programExists = _context.Programs.Any(p => p.ProgramId == ProgramId);
            if (!programExists)
            {
                ModelState.AddModelError("", "The selected ProgramId does not exist.");
                return RedirectToAction("ProgramStream", "ProjectLeader", new { id = ProgramId });
            }

            // Set the ProgramId in the QuizModel
            quizModel.ProgramId = ProgramId;

            // Save the QuizModel to the database
            _context.Quizzes.Add(quizModel);
            _context.SaveChanges();

            // Redirect to the CreateAssessment view, passing the NumberOfQuestions
            return RedirectToAction("CreateAssessment", new { id = quizModel.QuizId, numberOfQuestions = NumberOfQuestions });
        }
        public IActionResult CreateAssessment(int id, int numberOfQuestions)
        {
            var quiz = _context.Quizzes.Find(id);
            if (quiz == null)
            {
                return NotFound();
            }

            ViewBag.ProgramId = quiz.ProgramId; // Pass ProgramId to the view
            ViewBag.NumberOfQuestions = numberOfQuestions; // Set NumberOfQuestions from user input
            return View(quiz);
        }
        [HttpPost]
        public IActionResult SaveQuestions(QuizModel quizModel, List<QuestionModel> questions)
        {
            var quiz = _context.Quizzes.Include(q => q.Questions).FirstOrDefault(q => q.QuizId == quizModel.QuizId);

            if (quiz != null)
            {
                foreach (var question in questions)
                {
                    // Link question to the quiz and save it to the database
                    question.QuizId = quiz.QuizId;
                    _context.Questions.Add(question);
                    _context.SaveChanges(); // Save each question first to generate the QuestionId

                    // Now add the correct answer to AnswerModel
                    var correctAnswer = new AnswerModel
                    {
                        Answer = question.CorrectAnswer,  // This is the correct answer text from the form
                        QuestionId = question.QuestionId  // Associate it with the saved question
                    };
                    _context.Answers.Add(correctAnswer); // Save the answer to the Answer table
                }
                _context.SaveChanges(); // Save all answers at once
            }

            // Redirect to ProgramStream or another appropriate location
            return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = quiz.ProgramId });
        }
        public IActionResult ViewQuiz(int quizId)
        {
            var quiz = _context.Quizzes
                               .Include(q => q.Questions)
                               .ThenInclude(q => q.Answers)
                               .AsSplitQuery()
                               .FirstOrDefault(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz); // Pass the quiz model to the view
        }
        public async Task<IActionResult> StudentQuizView(int quizId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return NotFound("Quiz not found.");

            var enrollment = await _context.Enrollment
                .FirstOrDefaultAsync(e => e.TraineeId == currentUserId && e.ProgramId == quiz.ProgramId);

            if (enrollment == null) return Unauthorized("You are not enrolled in this program.");

            var result = await _context.StudentQuizResults
                .FirstOrDefaultAsync(r => r.QuizId == quizId && r.EnrollmentId == enrollment.EnrollmentId);

            if (result != null)
            {
                if (result.Remarks == "Pass")
                {
                    TempData["QuizErrorMessage"] = "You passed and cannot retake this quiz.";
                    return RedirectToAction("QuizResult", new { resultId = result.StudentQuizResultId });
                }
                else if (result.Retries >= 3)
                {
                    result.IsCompleted = true;
                    _context.Update(result);
                    await _context.SaveChangesAsync();

                    TempData["QuizErrorMessage"] = "Retry limit reached. Final result recorded.";
                    return RedirectToAction("QuizResult", new { resultId = result.StudentQuizResultId });
                }
            }

            ViewBag.EnrollmentId = enrollment.EnrollmentId;

            if (result?.Retries < 3 && result.Remarks == "Fail")
            {
                // Shuffle questions for retry
                quiz.Questions = quiz.Questions.OrderBy(q => Guid.NewGuid()).ToList();
            }

            return View(quiz);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int quizId, Dictionary<int, string> answers, int enrollmentId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return NotFound("Quiz not found.");

            var enrollment = await _context.Enrollment
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId && e.ProgramId == quiz.ProgramId);

            if (enrollment == null) return BadRequest("Invalid Enrollment.");

            var result = await _context.StudentQuizResults
                .Include(r => r.StudentAnswers) // Include existing answers for updating
                .FirstOrDefaultAsync(r => r.QuizId == quizId && r.EnrollmentId == enrollment.EnrollmentId);

            if (result == null)
            {
                result = new StudentQuizResultModel
                {
                    QuizId = quizId,
                    EnrollmentId = enrollmentId,
                    Retries = 0, // Set retries count
                };
                _context.StudentQuizResults.Add(result);
            }

            int rawScore = 0;

            foreach (var question in quiz.Questions)
            {
                var existingAnswer = result.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == question.QuestionId);

                // Normalize answers (convert to lowercase for case-insensitive comparison)
                var studentAnswerNormalized = answers.ContainsKey(question.QuestionId) ? answers[question.QuestionId].Trim().ToLower() : string.Empty;
                var correctAnswerNormalized = question.CorrectAnswer.Trim().ToLower();

                // Check if the student's answer is correct (case-insensitive comparison)
                var isCorrect = studentAnswerNormalized == correctAnswerNormalized;
                if (isCorrect) rawScore++;

                if (existingAnswer != null)
                {
                    // Update existing answer
                    existingAnswer.Answer = studentAnswerNormalized;
                    existingAnswer.IsCorrect = isCorrect;
                }
                else
                {
                    // Add new answer if not existing
                    result.StudentAnswers.Add(new StudentAnswerModel
                    {
                        QuestionId = question.QuestionId,
                        Answer = studentAnswerNormalized,
                        IsCorrect = isCorrect
                    });
                }
            }

            // Update score calculations
            result.RawScore = rawScore;
            result.TotalScore = quiz.Questions.Count;
            result.ComputedScore = Math.Round(((double)rawScore / result.TotalScore) * 62.5 + 37.5, 2);
            result.Remarks = result.ComputedScore >= 50 ? "Pass" : "Fail";

            if (result.Remarks == "Pass" || result.Retries >= 3)
            {
                result.IsCompleted = true;
            }
            else
            {
                result.Retries++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("QuizResult", new { resultId = result.StudentQuizResultId });
        }

        public async Task<IActionResult> QuizResult(int resultId)
        {
            // Ensure we include the Quiz along with StudentAnswers and related Questions
            var result = await _context.StudentQuizResults
                .Include(r => r.StudentAnswers)
                    .ThenInclude(sa => sa.Question) // Include the related questions for each answer
                .Include(r => r.Quiz)  // Ensure the Quiz is included here
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.StudentQuizResultId == resultId);

            if (result == null)
            {
                _logger.LogError("Quiz result not found for resultId: {ResultId}", resultId);
                return NotFound("Quiz result not found.");
            }

            if (result.Quiz == null)
            {
                _logger.LogError("Quiz information not found for resultId: {ResultId}", resultId);
                return NotFound("Quiz information not found.");
            }

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            var quiz = await _context.Quizzes
                                     .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz != null)
            {
                var programId = quiz.ProgramId;  // Get the ProgramId associated with the quiz

                // Remove related StudentQuizResults
                var studentQuizResults = _context.StudentQuizResults.Where(sqr => sqr.QuizId == quizId).ToList();
                _context.StudentQuizResults.RemoveRange(studentQuizResults);

                // Remove associated Questions and Answers
                var questions = _context.Questions.Where(q => q.QuizId == quizId).ToList();
                foreach (var question in questions)
                {
                    var answers = _context.Answers.Where(a => a.QuestionId == question.QuestionId).ToList();
                    _context.Answers.RemoveRange(answers);
                }
                _context.Questions.RemoveRange(questions);

                // Finally, remove the quiz itself
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Quiz and its related data deleted successfully.";

                // Store the ProgramId in TempData
                TempData["ProgramId"] = programId;

                // Redirect to the ProgramStream view for the related ProgramId
                return RedirectToAction("ProgramStream", "ProjectLeader");
            }
            else
            {
                TempData["Error"] = "Quiz not found.";
                return RedirectToAction("ProgramStream", "ProjectLeader");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuiz(QuizModel updatedQuiz, List<QuestionModel> updatedQuestions)
        {
            var existingQuiz = await _context.Quizzes
                                              .Include(q => q.Questions)
                                              .ThenInclude(q => q.Answers)
                                              .AsSplitQuery()
                                              .FirstOrDefaultAsync(q => q.QuizId == updatedQuiz.QuizId);

            if (existingQuiz == null)
            {
                return NotFound("Quiz not found.");
            }

            // Update Quiz title and description
            existingQuiz.QuizTitle = updatedQuiz.QuizTitle;
            existingQuiz.QuizDirection = updatedQuiz.QuizDirection;

            List<int> updatedQuestionIds = new List<int>();

            // Update Questions and Answers
            foreach (var updatedQuestion in updatedQuestions)
            {
                var existingQuestion = existingQuiz.Questions.FirstOrDefault(q => q.QuestionId == updatedQuestion.QuestionId);
                if (existingQuestion != null)
                {
                    if (existingQuestion.CorrectAnswer != updatedQuestion.CorrectAnswer)
                    {
                        // Track the question if the correct answer changed
                        updatedQuestionIds.Add(existingQuestion.QuestionId);
                    }

                    existingQuestion.Question = updatedQuestion.Question;
                    existingQuestion.CorrectAnswer = updatedQuestion.CorrectAnswer;

                    var existingAnswer = existingQuestion.Answers.FirstOrDefault();
                    if (existingAnswer != null)
                    {
                        existingAnswer.Answer = updatedQuestion.CorrectAnswer;
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Update StudentQuizResults based on the updated questions
            if (updatedQuestionIds.Any())
            {
                var affectedResults = _context.StudentQuizResults
                    .Include(r => r.StudentAnswers)
                    .Where(r => r.QuizId == updatedQuiz.QuizId)
                    .ToList();

                foreach (var result in affectedResults)
                {
                    int rawScore = 0;

                    foreach (var studentAnswer in result.StudentAnswers)
                    {
                        if (updatedQuestionIds.Contains(studentAnswer.QuestionId))
                        {
                            var correctAnswer = _context.Questions
                                .Where(q => q.QuestionId == studentAnswer.QuestionId)
                                .Select(q => q.CorrectAnswer)
                                .FirstOrDefault();

                            studentAnswer.IsCorrect = studentAnswer.Answer == correctAnswer;

                            if (studentAnswer.IsCorrect)
                            {
                                rawScore++;
                            }
                        }
                        else if (studentAnswer.IsCorrect)
                        {
                            rawScore++;
                        }
                    }

                    // Recompute the score
                    result.RawScore = rawScore;
                    result.TotalScore = result.StudentAnswers.Count;
                    result.ComputedScore = Math.Round(((double)rawScore / result.TotalScore) * 63.5 + 37.5, 2);
                    result.Remarks = result.ComputedScore >= 50 ? "Pass" : "Fail";
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Quiz and student results updated successfully!";
            return RedirectToAction("ViewQuiz", new { quizId = updatedQuiz.QuizId });
        }

    }
}
