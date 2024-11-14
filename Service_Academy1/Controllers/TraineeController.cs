using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace ServiceAcademy.Controllers
{
    public class TraineeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TraineeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyLearning()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];

            var userId = _userManager.GetUserId(User);

            var enrolledPrograms = await _context.Enrollment
                .Where(e => e.TraineeId == userId)
                .Include(e => e.ProgramsModel)
                    .ThenInclude(p => p.ProgramManagement)
                .AsSplitQuery()
                .AsNoTracking()
                .Select(e => new
                {
                    Program = e.ProgramsModel,
                    e.EnrollmentStatus,
                    e.ProgramStatus,
                    e.ReasonForDenial,
                    IsArchived = e.ProgramsModel.ProgramManagement.Any(pm => pm.IsArchived)
                })
                .ToListAsync();

            return View(enrolledPrograms);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProgram(int programId)
        {
            var userId = _userManager.GetUserId(User);

            var enrollment = await _context.Enrollment
                .Include(e => e.ProgramsModel)
                .ThenInclude(p => p.ProgramManagement)
                .FirstOrDefaultAsync(e => e.ProgramId == programId && e.TraineeId == userId);

            if (enrollment != null)
            {
                var isArchived = enrollment.ProgramsModel.ProgramManagement.Any(pm => pm.IsArchived);

                if (enrollment.EnrollmentStatus == "Denied" || isArchived)
                {
                    _context.Enrollment.Remove(enrollment);
                    await _context.SaveChangesAsync();
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

        public async Task<IActionResult> MyLearningStream(int programId)
        {
            if (programId <= 0)
            {
                TempData["Error"] = "Invalid Program ID";
                return RedirectToAction("MyLearning");
            }

            var userId = _userManager.GetUserId(User);

            var program = await _context.Programs
                .Include(p => p.Modules)
                .Include(p => p.Quizzes)
                    .ThenInclude(q => q.Questions)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProgramId == programId);

            if (program == null)
            {
                TempData["Error"] = "Program not found.";
                return RedirectToAction("MyLearning");
            }

            var viewModel = new MyLearningStreamViewModel
            {
                ProgramId = program.ProgramId,
                Title = program.Title,
                Description = program.Description,
                PhotoPath = program.PhotoPath,
                Modules = program.Modules.ToList(),
                Quizzes = program.Quizzes.ToList(),
                Enrollment = await _context.Enrollment
                    .Include(e => e.ProgramsModel)
                    .Where(e => e.TraineeId == userId)
                    .AsNoTracking()
                    .ToListAsync(),
                Evaluations = await _context.EvaluationResponses
                    .Include(x => x.EvaluationQuestion)
                    .Where(e => e.LearnerId == userId)
                    .AsNoTracking()
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RedirectToQuizOrResult(int quizId)
        {
            var userId = _userManager.GetUserId(User);

            var enrollment = await _context.Enrollment
                .Include(e => e.ProgramsModel.Quizzes)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TraineeId == userId && e.ProgramsModel.Quizzes.Any(q => q.QuizId == quizId));

            if (enrollment == null)
            {
                TempData["Error"] = "Enrollment not found for this quiz.";
                return RedirectToAction("MyLearningStream");
            }

            var quizResult = await _context.StudentQuizResults
                .AsNoTracking()
                .FirstOrDefaultAsync(sqr => sqr.QuizId == quizId && sqr.EnrollmentId == enrollment.EnrollmentId);

            if (quizResult != null)
            {
                return RedirectToAction("QuizResult", "Assessment", new { resultId = quizResult.StudentQuizResultId });
            }

            return RedirectToAction("StudentQuizView", "Assessment", new { quizId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
