using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Reflection;
using System;
using Newtonsoft.Json;

namespace ServiceAcademy.Controllers
{
    public class ProjectLeaderController : Controller
    {
        private readonly ILogger<ProjectLeaderController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProjectLeaderController(ILogger<ProjectLeaderController> logger, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;

        }
        public async Task<IActionResult> ProjectLeaderDashboard()
        {
            // Get the current user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch programs created by the logged-in instructor
            var programs = await _context.Programs
                                          .Where(p => p.ProjectLeaderId == currentUserId) // Filter by the current instructor's ID
                                          .Include(p => p.ProgramManagement) // Include the ProgramManagement relationship
                                          .Include(p => p.Enrollments)
                                          .AsSplitQuery()
                                          .ToListAsync();

            // Check if any programs are being retrieved
            if (programs == null || !programs.Any())
            {
                ViewData["Message"] = "No programs available.";
            }

            return View(programs);
        }
        public IActionResult ProgramStream(int programId)
        {
            // Check if ProgramId is passed via TempData, use it if not provided in the URL
            if (programId == 0 && TempData["ProgramId"] != null)
            {
                programId = (int)TempData["ProgramId"];
            }

            var programData = _context.Programs.FirstOrDefault(p => p.ProgramId == programId);
            if (programData == null)
            {
                return NotFound();
            }

            var program = _context.Programs
                .FirstOrDefault(p => p.ProgramId == programId);

            if (program == null)
            {
                TempData["Error"] = "Program not found.";
                return RedirectToAction("ProjectLeaderDashboard");
            }

            var modules = _context.Modules.Where(m => m.ProgramId == programId).ToList();
            var quizzes = _context.Quizzes.Where(q => q.ProgramId == programId)
                                           .Include(q => q.Questions)
                                           .ThenInclude(q => q.Answers)
                                           .AsSplitQuery()
                                           .ToList();

            var programManagement = _context.ProgramManagement.FirstOrDefault(pm => pm.ProgramId == programId);

            var viewModel = new ProgramStreamViewModel
            {
                ProgramId = program.ProgramId,
                Title = program.Title,
                Description = program.Description,
                PhotoPath = program.PhotoPath,
                Modules = modules,
                Quizzes = quizzes,
                IsArchived = programManagement?.IsArchived ?? false
            };

            return View(viewModel);
        }
        public IActionResult ProgramStreamManage(int programId)
        {
            _logger.LogInformation("Fetching enrolled trainees for Program ID: {ProgramId}", programId);

            // Fetch enrolled trainees for the specified program ID with a pending or approved status
            var enrolledTrainees = _context.Enrollment
                .Where(e => e.ProgramId == programId &&
                            (e.EnrollmentStatus == "Approved" || e.EnrollmentStatus == "Pending"))
                .Select(e => new EnrolleeViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    TraineeName = e.currentTrainee != null ? e.currentTrainee.UserName : "Unknown",
                    EnrollmentStatus = e.EnrollmentStatus,
                    ProgramStatus = e.ProgramStatus
                })
                .ToList();

            if (!enrolledTrainees.Any())
            {
                _logger.LogWarning("No enrolled trainees found for Program ID: {ProgramId}", programId);
            }
            else
            {
                _logger.LogInformation("Found {Count} enrolled trainees for Program ID: {ProgramId}", enrolledTrainees.Count, programId);
            }

            // Pass the ProgramId to the view (via ViewBag or ViewModel)
            ViewBag.ProgramId = programId;

            return View(enrolledTrainees);
        }
        public async Task<IActionResult> GetGrades(int enrollmentId, int programId)
        {
            var grades = await _context.StudentQuizResults
                .Where(sqr => sqr.EnrollmentId == enrollmentId && sqr.Quiz.ProgramId == programId)
                .Include(sqr => sqr.Quiz)
                .Select(sqr => new
                {
                    QuizTitle = sqr.Quiz.QuizTitle,
                    RawScore = sqr.RawScore,
                    TotalScore = sqr.TotalScore,
                    ComputedScore = sqr.ComputedScore,
                    Remarks = sqr.Remarks
                })
                .ToListAsync();

            // Log the query results
            Console.WriteLine("Grades fetched: " + JsonConvert.SerializeObject(grades));

            return Json(grades); // Ensure that the correct grades are returned
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEnrollment(int enrollmentId)
        {
            var enrollment = await _context.Enrollment.FindAsync(enrollmentId);
            if (enrollment == null) return NotFound();

            enrollment.EnrollmentStatus = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction("ProgramStreamManage", new { programId = enrollment.ProgramId });
        }

        [HttpPost]
        public async Task<IActionResult> DenyEnrollment(int enrollmentId, string reasonForDenial)
        {
            var enrollment = await _context.Enrollment.FindAsync(enrollmentId);
            if (enrollment == null) return NotFound();

            enrollment.EnrollmentStatus = "Denied";
            enrollment.ReasonForDenial = reasonForDenial; // Store the denial reason
            await _context.SaveChangesAsync();

            return RedirectToAction("ProgramStreamManage", new { programId = enrollment.ProgramId });
        }
        [HttpPost]
        public async Task<IActionResult> ActivateProgram(int programId, DateTime startDate, DateTime endDate)
        {
            var management = await _context.ProgramManagement.FirstOrDefaultAsync(pm => pm.ProgramId == programId);

            if (management == null)
            {
                management = new ProgramManagementModel
                {
                    ProgramId = programId,
                    StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                    IsArchived = false,
                    IsActive = true
                };

                _context.ProgramManagement.Add(management);
            }
            else
            {
                management.StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                management.EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                management.IsArchived = false;
                management.IsActive = true;
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Program activated successfully.";
            return RedirectToAction("ProjectLeaderDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateProgram(int programId)
        {
            var management = await _context.ProgramManagement.FirstOrDefaultAsync(pm => pm.ProgramId == programId);
            if (management != null)
            {
                management.EndDate = DateTime.UtcNow;
                management.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Program deactivated successfully.";
            }
            else
            {
                TempData["Error"] = "Program deactivation failed. Program not found.";
            }

            return RedirectToAction("ProjectLeaderDashboard");
        }



        [HttpPost]
        public async Task<IActionResult> ArchiveProgram(int programId)
        {
            var management = await _context.ProgramManagement.FirstOrDefaultAsync(pm => pm.ProgramId == programId);
            if (management != null)
            {
                management.IsArchived = true;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Program archived successfully.";
            }
            return RedirectToAction("ProjectLeaderDashboard");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProgram(int programId)
        {
            var program = await _context.Programs
                                        .FirstOrDefaultAsync(p => p.ProgramId == programId);

            if (program != null)
            {
                // Remove related quizzes and their dependent entities
                var quizzes = _context.Quizzes.Where(q => q.ProgramId == programId).ToList();
                foreach (var quiz in quizzes)
                {
                    // Remove associated StudentQuizResults
                    var studentQuizResults = _context.StudentQuizResults.Where(sqr => sqr.QuizId == quiz.QuizId).ToList();
                    _context.StudentQuizResults.RemoveRange(studentQuizResults);

                    // Remove associated Questions and Answers
                    var questions = _context.Questions.Where(q => q.QuizId == quiz.QuizId).ToList();
                    foreach (var question in questions)
                    {
                        var answers = _context.Answers.Where(a => a.QuestionId == question.QuestionId).ToList();
                        _context.Answers.RemoveRange(answers);
                    }
                    _context.Questions.RemoveRange(questions);

                    // Finally, remove the quiz
                    _context.Quizzes.Remove(quiz);
                }

                // Remove related modules (no need to load them using Include)
                var modules = _context.Modules.Where(m => m.ProgramId == programId);
                _context.Modules.RemoveRange(modules);

                // Remove related enrollments (no need to load them using Include)
                var enrollments = _context.Enrollment.Where(e => e.ProgramId == programId);
                _context.Enrollment.RemoveRange(enrollments);

                // Now remove the program itself
                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Program and its related data deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Program not found.";
            }

            return RedirectToAction("ProjectLeaderDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> UploadModule(int programId, string title, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file selected";
                return RedirectToAction("ProgramStream", new { programId });
            }

            // Determine next module number
            var moduleCount = _context.Modules.Count(m => m.ProgramId == programId);
            var moduleNumber = moduleCount + 1;

            // Format the title as "Module X: title" for saving
            var moduleTitle = $"Module {moduleNumber}: {title}";

            // Save the file
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploads, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Create module with the formatted title
            var module = new ModuleModel
            {
                ProgramId = programId,
                Title = moduleTitle,
                FilePath = "/uploads/" + file.FileName
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProgramStream", new { programId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModule(int moduleId, string moduleTitle, IFormFile file)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                TempData["Error"] = "Module not found.";
                return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
            }

            // Extract the "Module X" prefix from the existing title
            var moduleNumberPrefix = module.Title.Split(':')[0];
            module.Title = $"{moduleNumberPrefix}: {moduleTitle}";

            // Optionally update file if provided
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads, file.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                module.FilePath = "/uploads/" + file.FileName;
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Module updated successfully.";
            return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModule(int moduleId)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                TempData["Error"] = "Module not found.";
                return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
            }

            // Delete the module
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            // Re-fetch the modules, renumber them, and update titles
            var modules = _context.Modules
                                  .Where(m => m.ProgramId == module.ProgramId)
                                  .OrderBy(m => m.ModuleId)
                                  .ToList();

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Title = $"Module {i + 1}: {modules[i].Title.Split(':')[1].Trim()}";
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Module deleted and modules renumbered successfully.";
            return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
        }

    }
}


