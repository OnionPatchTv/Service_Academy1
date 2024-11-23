using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
using System.Security.Claims;
using Service_Academy1.Services;

namespace ServiceAcademy.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly LogSystemUsageService _logUsageService;
        public AssessmentController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment environment, LogSystemUsageService logUsageService)
        {
            (_logger, _context, _environment, _logUsageService) = (logger, context, environment, logUsageService);
        }
        #region Quiz Management
        [HttpPost]
        public IActionResult CreateQuiz(QuizModel quizModel, int NumberOfQuestions, int ProgramId)
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
            TempData["ProgramStreamSuccessMessage"] = "Quiz created successfully";
            // Redirect to ProgramStream or another appropriate location
            return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = quiz.ProgramId });
        }
        public IActionResult ViewQuiz(int quizId)
        {
            var quiz = _context.Quizzes
                               .Include(q => q.ProgramsModel)
                               .Include(q => q.Questions)
                               .ThenInclude(q => q.Answers)
                               .AsSplitQuery()
                               .FirstOrDefault(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return NotFound();
            }
            ViewBag.ProgramTitle = quiz.ProgramsModel.Title;
            return View(quiz); // Pass the quiz model to the view
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
                var studentQuizResults = _context.TraineeQuizResults.Where(sqr => sqr.QuizId == quizId).ToList();
                _context.TraineeQuizResults.RemoveRange(studentQuizResults);

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

                TempData["ProgramStreamSuccessMessage"] = "Quiz and its related data deleted successfully.";

                // Store the ProgramId in TempData
                TempData["ProgramId"] = programId;

                // Redirect to the ProgramStream view for the related ProgramId
                return RedirectToAction("ProgramStream", "ProjectLeader");
            }
            else
            {
                TempData["ProgramStreamErrorMessage"] = "Quiz not found.";
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
                var affectedResults = _context.TraineeQuizResults
                    .Include(r => r.TraineeAnswers)
                    .Where(r => r.QuizId == updatedQuiz.QuizId)
                    .ToList();

                foreach (var result in affectedResults)
                {
                    int rawScore = 0;

                    foreach (var studentAnswer in result.TraineeAnswers)
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
                    result.TotalScore = result.TraineeAnswers.Count;
                    result.ComputedScore = Math.Round(((double)rawScore / result.TotalScore) * 62.5 + 37.5, 2);
                    result.Remarks = result.ComputedScore >= 50 ? "Pass" : "Fail";
                }

                await _context.SaveChangesAsync();
            }

            TempData["ProgramStreamSuccessMessage"] = "Quiz and student results updated successfully!";
            return RedirectToAction("ViewQuiz", new { quizId = updatedQuiz.QuizId });
        }
        #endregion

        #region Student Quiz Management
        public async Task<IActionResult> StudentQuizView(int quizId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = await _context.Quizzes
                .Include(q => q.ProgramsModel)
                .Include(q => q.Questions)
                .AsSplitQuery()
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null) return NotFound("Quiz not found.");

            var enrollment = await _context.Enrollment
                .FirstOrDefaultAsync(e => e.TraineeId == currentUserId && e.ProgramId == quiz.ProgramId);

            if (enrollment == null) return Unauthorized("You are not enrolled in this program.");

            var result = await _context.TraineeQuizResults
                .FirstOrDefaultAsync(r => r.QuizId == quizId && r.EnrollmentId == enrollment.EnrollmentId);

            if (result != null)
            {
                if (result.Remarks == "Pass")
                {
                    TempData["MyLearningStreamErrorMessage"] = "You passed and cannot retake this quiz.";
                    return RedirectToAction("QuizResult", new { resultId = result.TraineeQuizResultId });
                }
                else if (result.Retries >= 3)
                {
                    result.IsCompleted = true;
                    _context.Update(result);
                    await _context.SaveChangesAsync();

                    TempData["MyLearningStreamErrorMessage"] = "Retry limit reached. Final result recorded.";
                    return RedirectToAction("QuizResult", new { resultId = result.TraineeQuizResultId });
                }
            }

            ViewBag.EnrollmentId = enrollment.EnrollmentId;

            if (result?.Retries < 3 && result.Remarks == "Fail")
            {
                // Shuffle questions for retry
                quiz.Questions = quiz.Questions.OrderBy(q => Guid.NewGuid()).ToList();
            }
            ViewBag.ProgramTitle = quiz.ProgramsModel.Title;
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

            var result = await _context.TraineeQuizResults
                .Include(r => r.TraineeAnswers) // Include existing answers for updating
                .FirstOrDefaultAsync(r => r.QuizId == quizId && r.EnrollmentId == enrollment.EnrollmentId);

            if (result == null)
            {
                result = new TraineeQuizResultModel
                {
                    QuizId = quizId,
                    EnrollmentId = enrollmentId,
                    Retries = 0, // Set retries count
                };
                _context.TraineeQuizResults.Add(result);
            }

            int rawScore = 0;

            foreach (var question in quiz.Questions)
            {
                var existingAnswer = result.TraineeAnswers.FirstOrDefault(sa => sa.QuestionId == question.QuestionId);

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
                    result.TraineeAnswers.Add(new TraineeAnswerModel
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
            await _logUsageService.LogSystemUsageAsync(enrollment.TraineeId, "QuizSubmission", quizId);

            TempData["MyLearningStreamSuccessMessage"] = "Quiz finished and recorded.";
            return RedirectToAction("QuizResult", new { resultId = result.TraineeQuizResultId });
        }

        public async Task<IActionResult> QuizResult(int resultId)
        {
            // Ensure we include the Quiz along with StudentAnswers and related Questions
            var result = await _context.TraineeQuizResults
                .Include(r => r.TraineeAnswers)
                    .ThenInclude(sa => sa.Question) // Include the related questions for each answer
                .Include(r => r.Quiz)// Ensure the Quiz is included here
                .ThenInclude(q => q.ProgramsModel)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.TraineeQuizResultId == resultId);

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
            var program = result.Quiz.ProgramsModel;
            ViewBag.ProgramTitle = program.Title;
            ViewBag.ProgramId = program.ProgramId;
            return View(result);
        }
        #endregion

        #region Activiy Management
        [HttpPost]
        public IActionResult CreateActivity(ActivitiesModel activityModel, int ProgramId)
        {
            // Check if ProgramId exists
            var programExists = _context.Programs.Any(p => p.ProgramId == ProgramId);
            if (!programExists)
            {
                ModelState.AddModelError("", "The selected ProgramId does not exist.");
                return RedirectToAction("ProgramStream", "ProjectLeader", new { id = ProgramId });
            }

            // Set the ProgramId in the ActivitiesModel
            activityModel.ProgramId = ProgramId;

            // Add created date in UTC format
            activityModel.CreatedAt = DateTime.UtcNow;

            // Save the ActivitiesModel to the database
            _context.Activities.Add(activityModel);
            _context.SaveChanges();

            TempData["ProgramStreamSuccessMessage"] = "Successfully created an Activity.";
            return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = ProgramId });
        }

        public IActionResult ViewActivity(int activitiesId)
        {
            // Fetch the activity along with the enrolled trainees' activities
            var activity = _context.Activities
                .Include(a => a.TraineeActivities)
                .ThenInclude(ta => ta.Enrollment)
                .ThenInclude(e => e.CurrentTrainee)
                .FirstOrDefault(a => a.ActivitiesId == activitiesId);

            if (activity == null)
            {
                return NotFound();
            }

            // Create a ViewModel to pass the data to the view
            var viewModel = new ActivityViewModel
            {
                Activity = activity,
                TraineeActivities = activity.TraineeActivities.ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateActivity(int activitiesId, string activitiesTitle, string activityDirection, int totalScore)
        {
            var activity = await _context.Activities.FindAsync(activitiesId);
            if (activity == null)
            {
                TempData["ProgramStreamErrorMessage"] = "Activity not found.";
                return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = activity.ProgramId });
            }

            // Update activity details
            activity.ActivitiesTitle = activitiesTitle;
            activity.ActivityDirection = activityDirection;
            activity.TotalScore = totalScore;

            await _context.SaveChangesAsync();

            TempData["ProgramStreamSuccessMessage"] = "Activity updated successfully.";
            return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = activity.ProgramId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteActivity(int activitiesId)
        {
            var activity = await _context.Activities
                          .Include(a => a.TraineeActivities)
                           .FirstOrDefaultAsync(a => a.ActivitiesId == activitiesId);

            if (activity == null)
            {
                TempData["ProgramStreamErrorMessage"] = "Activity not found.";
                return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = activity.ProgramId });
            }

            // Delete the activity
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            TempData["ProgramStreamSuccessMessage"] = "Activity and its corresponding data are deleted successfully.";
            return RedirectToAction("ProgramStream", "ProjectLeader", new { programId = activity.ProgramId });
        }

        #endregion

        #region Student Activity Management
        [HttpGet]
        public IActionResult GetSubmissionDetails(int activitiesId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var submission = _context.TraineeActivities
                                     .FirstOrDefault(ta => ta.ActivitiesId == activitiesId && ta.Enrollment.TraineeId == currentUserId);

            if (submission == null)
            {
                return NotFound();
            }

            return Json(new { submission.FilePath, submission.LinkPath });
        }

        public async Task<IActionResult> SubmitActivity(int activitiesId, string? submissionLink, IFormFile? submissionFile)
        {
            // Retrieve the activity and validate
            var activity = _context.Activities.Include(a => a.ProgramsModel)
                                              .FirstOrDefault(a => a.ActivitiesId == activitiesId);
            if (activity == null)
            {
                return BadRequest("Activity not found.");
            }

            // Retrieve the enrollment record for the current trainee
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you're using ASP.NET Identity
            var enrollment = _context.Enrollment.FirstOrDefault(e => e.ProgramId == activity.ProgramId && e.TraineeId == currentUserId);

            if (enrollment == null)
            {
                return BadRequest("You are not enrolled in this program.");
            }

            // Check if a submission already exists
            var existingSubmission = _context.TraineeActivities
                                             .FirstOrDefault(ta => ta.ActivitiesId == activitiesId && ta.EnrollmentId == enrollment.EnrollmentId);

            string filePath = string.Empty;

            // Upload new file if provided
            if (submissionFile != null)
            {
                string fileName = $"{Path.GetFileNameWithoutExtension(submissionFile.FileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(submissionFile.FileName)}";
                filePath = Path.Combine("ActivityUploads", fileName); // Relative path
                string fullPath = Path.Combine(_environment.WebRootPath, filePath); // Absolute path

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await submissionFile.CopyToAsync(fileStream);
                }
            }

            if (existingSubmission != null)
            {
                // Update existing submission
                if (!string.IsNullOrEmpty(filePath))
                    existingSubmission.FilePath = filePath;

                if (!string.IsNullOrEmpty(submissionLink))
                    existingSubmission.LinkPath = submissionLink;

                existingSubmission.SubmittedAt = DateTime.UtcNow;
                _context.TraineeActivities.Update(existingSubmission);
            }
            else
            {
                // Create a new TraineeActivities record
                var traineeActivity = new TraineeActivitiesModel
                {
                    ActivitiesId = activitiesId,
                    EnrollmentId = enrollment.EnrollmentId, // Link to the correct enrollment
                    FilePath = !string.IsNullOrEmpty(filePath) ? filePath : "No Document",
                    LinkPath = !string.IsNullOrEmpty(submissionLink) ? submissionLink : "No Link Pasted",
                    RawScore = 0,
                    ComputedScore = 0,
                    IsCompleted = true,
                    SubmittedAt = DateTime.UtcNow
                };

                _context.TraineeActivities.Add(traineeActivity);
            }

            await _context.SaveChangesAsync();
            TempData["MyLearningStreamSuccessMessage"] = "Successfully uploaded an activity.";
            return RedirectToAction("MyLearningStream", "Trainee", new { programId = activity.ProgramId });
        }

        [HttpGet]
        public IActionResult GetScores(int activitiesId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var submission = _context.TraineeActivities
                                     .FirstOrDefault(ta => ta.ActivitiesId == activitiesId && ta.Enrollment.TraineeId == currentUserId);

            if (submission == null)
            {
                return Json(new { rawScore = 0, computedScore = 0 });
            }

            return Json(new { rawScore = submission.RawScore, computedScore = submission.ComputedScore });
        }

        [HttpPost]
        public IActionResult UpdateRawScore(int traineeActivityId, int rawScore)
        {
            // Fetch the specific TraineeActivity record
            var traineeActivity = _context.TraineeActivities
                .Include(t => t.Activities) // Ensure the related Activity is included
                .FirstOrDefault(t => t.TraineeActivityId == traineeActivityId);

            if (traineeActivity == null)
            {
                return NotFound("Trainee activity not found.");
            }

            // Update the Raw Score
            traineeActivity.RawScore = rawScore;

            // Recalculate the Computed Score
            if (traineeActivity.Activities != null)
            {
                double totalScore = traineeActivity.Activities.TotalScore;
                traineeActivity.ComputedScore = (int)Math.Round(((double)rawScore / totalScore) * 62.5 + 37.5);
            }

            // Save changes to the database
            _context.SaveChanges();

            // Redirect back to the activity view
            return RedirectToAction("ViewActivity", new { activitiesId = traineeActivity.ActivitiesId });
        }
        public IActionResult ViewSubmission(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return NotFound("File path is missing.");
            }

            // Resolve the absolute path based on your hosting environment
            string absolutePath = Path.Combine(_environment.WebRootPath, filePath);

            // Validate if the file exists
            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound("File not found.");
            }

            string extension = Path.GetExtension(absolutePath).ToLower();

            if (extension == ".pdf")
            {
                // Serve PDF files
                var fileBytes = System.IO.File.ReadAllBytes(absolutePath);
                return File(fileBytes, "application/pdf");
            }
            else if (extension == ".txt" || extension == ".html")
            {
                // Serve text or HTML files
                string content = System.IO.File.ReadAllText(absolutePath);
                return Content(content, extension == ".html" ? "text/html" : "text/plain");
            }
            else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
            {
                // Serve image files
                var fileBytes = System.IO.File.ReadAllBytes(absolutePath);
                return File(fileBytes, $"image/{extension.TrimStart('.')}");
            }

            // Fallback for unsupported types
            return BadRequest("Unsupported file type.");
        }


        #endregion
    }
}
