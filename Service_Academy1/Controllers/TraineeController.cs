using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ServiceAcademy.Controllers
{
    public class TraineeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TraineeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult MyLearning()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Current user's ID
            var enrollments = _context.Enrollment
                .Include(e => e.ProgramsModel)
                .Include(e => e.ProgramsModel.Modules)
                .Include(e => e.ProgramsModel.Quizzes)
                .Include(e => e.ProgramsModel.Activities)
                .Include(e => e.ProgramsModel.EvaluationQuestions)
                .Where(e => e.TraineeId == userId)
                .Select(e => new MyLearningViewModel
                {
                    ProgramId = e.ProgramId,
                    Title = e.ProgramsModel.Title,
                    Agenda = e.ProgramsModel.Agenda,
                    PhotoPath = e.ProgramsModel.PhotoPath,
                    EnrollmentStatus = e.EnrollmentStatus,
                    ProgramStatus = e.ProgramStatus,
                    ReasonForDenial = e.ReasonForDenial,
                    IsArchived = e.ProgramsModel.ProgramManagement.Any(pm => pm.IsArchived),

                    // Calculate progress
                    ModulesProgress = e.ProgramsModel.Modules.Any()
                    ? Math.Round(e.ProgramsModel.Modules.Count(m => e.TraineeModuleResults.Any(tmr => tmr.ModuleId == m.ModuleId && tmr.IsCompleted)) * 15.0 / e.ProgramsModel.Modules.Count(), 2)
                    : 0,
                        ActivitiesProgress = e.ProgramsModel.Activities.Any()
                    ? Math.Round(e.ProgramsModel.Activities.Count(a => e.TraineeActivities.Any(ta => ta.ActivitiesId == a.ActivitiesId && ta.IsCompleted)) * 40.0 / e.ProgramsModel.Activities.Count(), 2)
                    : 0,
                        QuizzesProgress = e.ProgramsModel.Quizzes.Any()
                    ? Math.Round(e.ProgramsModel.Quizzes.Sum(q =>
                        e.TraineeQuizResults.Any(tqr => tqr.QuizId == q.QuizId && tqr.IsCompleted) ? 1.0 :
                        e.TraineeQuizResults.Any(tqr => tqr.QuizId == q.QuizId) ? 0.5 : 0) * 30.0 / e.ProgramsModel.Quizzes.Count(), 2)
                    : 0,
                        EvaluationProgress = e.ProgramsModel.EvaluationQuestions.Any()
                    ? Math.Round(e.ProgramsModel.EvaluationQuestions.All(eq => e.EvaluationResponses.Any(er => er.EvaluationQuestionId == eq.EvaluationQuestionId)) ? 15.0 : 0, 2)
                    : 0
                }).ToList();

            return View(enrollments);
        }

        [HttpPost]
        public IActionResult DeleteProgram(int programId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var enrollment = _context.Enrollment
                .Include(e => e.ProgramsModel)
                .ThenInclude(p => p.ProgramManagement)
                .AsSplitQuery()
                .FirstOrDefault(e => e.ProgramId == programId && e.TraineeId == userId);

            if (enrollment != null)
            {
                // Check if the program is archived
                var isArchived = enrollment.ProgramsModel.ProgramManagement.Any(pm => pm.IsArchived);

                if (enrollment.EnrollmentStatus == "Denied" || isArchived)
                {
                    // Allow deletion if status is Denied or the program is archived
                    _context.Enrollment.Remove(enrollment);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = isArchived
                        ? "Enrollment deleted successfully as the program is archived."
                        : "Enrollment deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot delete enrollment. It can only be deleted if denied or archived.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Enrollment not found or invalid program ID.";
            }

            return RedirectToAction("MyLearning");
        }

        public IActionResult MyLearningStream(int programId)
        {
            if (programId <= 0)
            {
                TempData["Error"] = "Invalid Program ID";
                return RedirectToAction("MyLearning");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var program = _context.Programs
                .Include(p => p.Modules)
                .Include(a => a.Activities)
                    .ThenInclude(a => a.TraineeActivities)
                .Include(p => p.Quizzes)
                    .ThenInclude(q => q.Questions)
                .AsSplitQuery()
                .FirstOrDefault(p => p.ProgramId == programId);

            if (program == null)
            {
                TempData["ErrorMessage"] = "Program not found.";
                return RedirectToAction("MyLearning");
            }

            var enrollmentId = _context.Enrollment
                .Where(e => e.TraineeId == userId && e.ProgramId == programId)
                .Select(e => e.EnrollmentId)
                .FirstOrDefault();

            var evaluations = _context.EvaluationResponses
                .Where(e => e.EnrollmentId == enrollmentId)
                .Include(x => x.EvaluationQuestions)
                .AsSplitQuery()
                .ToList();

            var traineeModuleResults = _context.TraineeModuleResults
                .Where(tmr => tmr.EnrollmentId == enrollmentId)
                .ToList();
            var traineeQuizResults = _context.TraineeQuizResults
                .Where(tmr => tmr.EnrollmentId == enrollmentId)
                .ToList();

            var viewModel = new MyLearningStreamViewModel
            {
                ProgramId = program.ProgramId,
                Title = program.Title,
                Description = program.Description,
                PhotoPath = program.PhotoPath,
                Modules = program.Modules.ToList(),
                Quizzes = program.Quizzes.ToList(),
                Activities = program.Activities.ToList(),
                Enrollment = _context.Enrollment
                                  .Where(e => e.TraineeId == userId && e.ProgramId == programId)
                                  .Include(e => e.ProgramsModel)
                                  .AsSplitQuery()
                                  .ToList(),
                Evaluations = evaluations,
                TraineeModuleResults = traineeModuleResults,
                 TraineeQuizResults = traineeQuizResults
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult MarkAsRead(int programId, int moduleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user (trainee)

            // Get the trainee's enrollment ID for the current program
            var enrollment = _context.Enrollment
                .FirstOrDefault(e => e.TraineeId == userId && e.ProgramId == programId);

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("MyLearningStream", new { programId = programId });
            }

            // Check if there's already an existing TraineeModuleResult record
            var traineeModuleResult = _context.TraineeModuleResults
                .FirstOrDefault(tmr => tmr.EnrollmentId == enrollment.EnrollmentId && tmr.ModuleId == moduleId);

            if (traineeModuleResult == null)
            {
                // Insert a new record into the TraineeModuleResult table
                traineeModuleResult = new TraineeModuleResult
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    ModuleId = moduleId,
                    IsCompleted = true
                };

                _context.TraineeModuleResults.Add(traineeModuleResult);
            }
            else
            {
                // If the record exists, just update the IsCompleted status to true
                traineeModuleResult.IsCompleted = true;
            }

            // Save changes to the database
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Module marked as read!";
            return RedirectToAction("MyLearningStream", new { programId = programId });
        }

        [HttpGet]
        public IActionResult RedirectToQuizOrResult(int quizId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the enrollment for the current user and the specified quiz
            var enrollment = _context.Enrollment
                .FirstOrDefault(e => e.TraineeId == userId &&
                                     e.ProgramsModel.Quizzes.Any(q => q.QuizId == quizId));

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found for this quiz.";
                return RedirectToAction("MyLearningStream");
            }

            // Check for an existing quiz result for this enrollment and quiz
            var quizResult = _context.TraineeQuizResults
                .FirstOrDefault(sqr => sqr.QuizId == quizId && sqr.EnrollmentId == enrollment.EnrollmentId);

            if (quizResult != null)
            {
                // Redirect to QuizResult using the StudentQuizResultId
                return RedirectToAction("QuizResult", "Assessment", new { resultId = quizResult.TraineeQuizResultId });
            }
            else
            {
                // If no result exists, redirect to StudentQuizView for answering
                return RedirectToAction("StudentQuizView", "Assessment", new { quizId = quizId });
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}

