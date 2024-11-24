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
using System.Net.Mail;
using System.Net;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using Service_Academy1.Services;
using Org.BouncyCastle.Ocsp;
using iText.IO.Font;
using Org.BouncyCastle.Tls;

namespace ServiceAcademy.Controllers
{
    public class ProjectLeaderController : Controller
    {
        private readonly ILogger<ProjectLeaderController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly EmailService _emailService;

        public ProjectLeaderController(ILogger<ProjectLeaderController> logger, ApplicationDbContext context, IWebHostEnvironment environment, EmailService emailService)
        {
            (_logger, _context, _environment, _emailService) = (logger, context, environment, emailService);

        }
        #region ProjectLeader Dashboard Management
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
                ViewData["ProjectLeaderErrorMessage"] = "No programs available.";
            }

            return View(programs);
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
            TempData["ProjectLeaderSuccessMessage"] = "Program activated successfully.";
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
                TempData["ProjectLeaderSuccessMessage"] = "Program deactivated successfully.";
            }
            else
            {
                TempData["ProjectLeaderErrorMessage"] = "Program deactivation failed. Program not found.";
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
                TempData["ProjectLeaderSuccessMessage"] = "Program archived successfully.";
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
                    var studentQuizResults = _context.TraineeQuizResults.Where(sqr => sqr.QuizId == quiz.QuizId).ToList();
                    _context.TraineeQuizResults.RemoveRange(studentQuizResults);

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

                TempData["ProjectLeaderSuccessMessage"] = "Program and its related data deleted successfully.";
            }
            else
            {
                TempData["ProjectLeaderErrorMessage"] = "Program not found.";
            }

            return RedirectToAction("ProjectLeaderDashboard");
        }

        #endregion
        #region ProgramStream Management
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
                TempData["ProgramStreamErrorMessage"] = "Program not found.";
                return RedirectToAction("ProjectLeaderDashboard");
            }

            var modules = _context.Modules.Where(m => m.ProgramId == programId).ToList();
            var activities = _context.Activities.Where(a => a.ProgramId == programId)
                                          .Include(a => a.TraineeActivities)
                                           .ToList();
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
                Activities = activities,
                IsArchived = programManagement?.IsArchived ?? false
            };

            return View(viewModel);
        }
        #region Module Management
        [HttpPost]
        public async Task<IActionResult> UploadModule(int programId, string title, IFormFile file, string moduleDescription, string linkPath = "No Link Available")
        {
            if (file == null || file.Length == 0)
            {
                TempData["ProgramStreamErrorMessage"] = "No file selected";
                return RedirectToAction("ProgramStream", new { programId });
            }
            if (title.Length > 255)
            {
                ModelState.AddModelError("title", "Module title is too long (maximum 255 characters).");
            }
            if (moduleDescription.Length > 1000)
            {
                ModelState.AddModelError("moduleDescription", "Module description is too long (maximum 1000 characters).");
            }
            var moduleCount = _context.Modules.Count(m => m.ProgramId == programId);
            var moduleNumber = moduleCount + 1;
            var moduleTitle = $"Module {moduleNumber}: {title}";

            var uploads = Path.Combine(_environment.WebRootPath, "ModuleUploads");
            var filePath = Path.Combine(uploads, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var module = new ModuleModel
            {
                ProgramId = programId,
                Title = moduleTitle,
                FilePath = "/ModuleUploads/" + file.FileName,
                LinkPath = string.IsNullOrWhiteSpace(linkPath) ? "No Link Available" : linkPath,
                ModuleDescription = moduleDescription
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            TempData["ProgramStreamSuccessMessage"] = "Module uploaded successfully.";
            return RedirectToAction("ProgramStream", new { programId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModule(int moduleId, string moduleTitle, IFormFile file, string moduleDescription, string linkPath)
        {
            if (moduleTitle.Length > 50)
            {
                ModelState.AddModelError("title", "Module title is too long (maximum 255 characters).");
            }
            if (moduleDescription.Length > 500)
            {
                ModelState.AddModelError("moduleDescription", "Module description is too long (maximum 500 characters).");
            }
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                TempData["ProgramStreamErrorMessage"] = "Module not found.";
                return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
            }

            var moduleNumberPrefix = module.Title.Split(':')[0];
            module.Title = $"{moduleNumberPrefix}: {moduleTitle}";
            module.LinkPath = string.IsNullOrWhiteSpace(linkPath) ? "No Link Available" : linkPath;
            module.ModuleDescription = moduleDescription;

            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "ModuleUploads");
                var filePath = Path.Combine(uploads, file.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                module.FilePath = "/ModuleUploads/" + file.FileName;
            }

            await _context.SaveChangesAsync();
            TempData["ProgramStreamSuccessMessage"] = "Module updated successfully.";
            return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModule(int moduleId)
        {
            var module = await _context.Modules.FindAsync(moduleId);
            if (module == null)
            {
                TempData["ProgramStreamErrorMessage"] = "Module not found.";
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

            TempData["ProgramStreamSuccessMessage"] = "Module deleted and renumbered successfully.";
            return RedirectToAction("ProgramStream", new { programId = module.ProgramId });
        }
        #endregion
        #endregion

        #region ProgramStreamManage Management
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
                    TraineeName = e.CurrentTrainee != null ? e.CurrentTrainee.FullName : "Unknown",
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
        [HttpPost]
        public async Task<IActionResult> ApproveCompletion(int enrollmentId)
        {
            try
            {
                var enrollment = await _context.Enrollment
                    .Include(e => e.CurrentTrainee)
                    .Include(e => e.ProgramsModel)
                    .ThenInclude(p => p.CurrentProjectLeader)
                    .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

                if (enrollment == null)
                {
                    return NotFound();
                }

                enrollment.ProgramStatus = "Complete";
                enrollment.StatusDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                string certificateFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates", $"{enrollment.EnrollmentId}.pdf");

                // Check if the certificate file exists
                if (!System.IO.File.Exists(certificateFilePath))
                {
                    // Generate the certificate if it doesn't exist
                    string newCertificatePath = await GenerateCertificateAsync(enrollment);

                    if (string.IsNullOrEmpty(newCertificatePath))
                    {
                        throw new Exception("Certificate generation failed.");
                    }

                    certificateFilePath = newCertificatePath;
                }

                // Return both the relative web path and absolute file path
                string certificateWebPath = $"/certificates/{enrollment.EnrollmentId}.pdf";
                return Json(new { certificateWebPath, certificateFilePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private async Task<string> GenerateCertificateAsync(EnrollmentModel enrollment)
        {
            string traineeName = enrollment.CurrentTrainee?.FullName ?? "N/A";
            string programName = enrollment.ProgramsModel?.Title ?? "N/A";
            string projectLeaderName = enrollment.ProgramsModel?.CurrentProjectLeader?.FullName ?? "N/A";
            DateTime generatedDate = DateTime.UtcNow;
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates", $"{enrollment.EnrollmentId}.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            try
            {
                // Ensure the template file exists
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "certificate_template.pdf");
                if (!System.IO.File.Exists(templatePath))
                {
                    throw new FileNotFoundException("Certificate template not found.");
                }

                using (var reader = new PdfReader(templatePath))
                using (var writer = new PdfWriter(outputPath))
                using (var pdfDoc = new PdfDocument(reader, writer))
                {
                    var page = pdfDoc.GetFirstPage();
                    var canvas = new PdfCanvas(page);

                    // Load Poppins fonts
                    PdfFont poppinsBold = PdfFontFactory.CreateFont("wwwroot/Resources/Fonts/Poppins-Bold.ttf", PdfEncodings.IDENTITY_H);
                    PdfFont poppinsRegular = PdfFontFactory.CreateFont("wwwroot/Resources/Fonts/Poppins-Regular.ttf", PdfEncodings.IDENTITY_H);


                    canvas.BeginText()

                        // Trainee Name (Poppins Bold)
                        .SetFontAndSize(poppinsBold, 28) // Use Poppins-Bold font with a larger size
                        .SetTextMatrix((float)347.5, (float)339.3)
                        .ShowText($"{traineeName}")

                        // Program Name (Poppins Regular)
                        .SetFontAndSize(poppinsRegular, 22) // Use Poppins-Regular font with a smaller size
                        .SetTextMatrix((float)347.5, (float)263.0)
                        .ShowText($"{programName}")

                        // Project Leader Name (Poppins Regular)
                        .SetFontAndSize(poppinsRegular, 20) // Use Poppins-Regular font
                        .SetTextMatrix((float)526.0, (float)126.0)
                        .ShowText($"{projectLeaderName}")

                        // Date (Poppins Regular)
                        .SetFontAndSize(poppinsRegular, 16) // Use Poppins-Regular font
                        .SetTextMatrix((float)347.5, (float)232.5)
                        .ShowText($"{generatedDate:MMMM dd, yyyy}")

                        // Certificate ID (Poppins Regular)  ((float)126.0, (float)39.0)   (- = up , - = left)
                        .SetFontAndSize(poppinsRegular, 12) // Use Poppins-Regular font
                    .SetTextMatrix((float)126.0, (float)39.0)
                        .ShowText($"{projectLeaderName}")

                        .EndText();
                }

                    // Create a new CertificateModel to store in the database
                    var certificate = new CertificateModel
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    CertificatePath = outputPath, // Store the path where the certificate was saved
                    GeneratedDate = generatedDate // Store the date when it was generated
                };

                // Save the certificate to the database
                _context.Certificates.Add(certificate);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Log the exception and rethrow or handle accordingly
                throw new Exception($"Error generating certificate: {ex.Message}");
            }

            return outputPath;
        }
        [HttpPost]
        public async Task<IActionResult> SendCertificateEmail(int enrollmentId, string certificatePath)
        {
            try
            {
                var enrollment = await _context.Enrollment
                    .Include(e => e.CurrentTrainee)
                    .Include(e => e.ProgramsModel)
                    .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

                if (enrollment == null || enrollment.CurrentTrainee == null)
                {
                    return Json(new { success = false, message = "Trainee not found." });
                }

                // Verify the certificate file exists
                if (!System.IO.File.Exists(certificatePath))
                {
                    return Json(new { success = false, message = "Certificate file not found." });
                }

                // Prepare the email content
                var traineeEmail = enrollment.CurrentTrainee.Email;
                var subject = "Your Program Completion Certificate";
                var body = $"Dear {enrollment.CurrentTrainee.FullName},<br><br>Congratulations on completing the program!<br><br>Your certificate is attached below.<br><br>Best regards,<br>{enrollment.ProgramsModel?.CurrentProjectLeader?.FullName}";

                byte[] certificateContent = System.IO.File.ReadAllBytes(certificatePath);
                string attachmentFileName = $"{enrollment.ProgramsModel?.Title}_Certificate.pdf";

                // Send the email with the attachment
                await _emailService.SendEmailWithAttachmentAsync(
                    traineeEmail,
                    subject,
                    body,
                    null,  // Optional: reply-to email
                    certificateContent,
                    attachmentFileName
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult GetTraineeActivities(int enrollmentId, int programId)
        {
            var activities = _context.TraineeActivities
                .Where(ta => ta.EnrollmentId == enrollmentId && ta.Activities.ProgramId == programId)
                .Select(ta => new
                {
                    ActivityTitle = ta.Activities.ActivitiesTitle,
                    RawScore = ta.RawScore,
                    TotalScore = ta.Activities.TotalScore,
                    ComputedScore = ta.ComputedScore,
                })
                .ToList();

            return Json(activities);
        }

        public async Task<IActionResult> GetGrades(int enrollmentId, int programId)
        {
            var grades = await _context.TraineeQuizResults
                .Where(sqr => sqr.EnrollmentId == enrollmentId && sqr.Quiz.ProgramId == programId)
                .Include(sqr => sqr.Quiz)
                .Select(sqr => new
                {
                    QuizTitle = sqr.Quiz.QuizTitle,
                    RawScore = sqr.RawScore,
                    TotalScore = sqr.TotalScore,
                    Retries = sqr.Retries,
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

            TempData["ProgramStreamManageSuccessMessage"] = "Successfully approved enrollment.";
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

            TempData["ProgramStreamManageSuccessMessage"] = "Successfully denied enrollment.";
            return RedirectToAction("ProgramStreamManage", new { programId = enrollment.ProgramId });
        }
        #endregion

        #region Announcement
        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(AnnouncementModel announcement)
        {
            if (ModelState.IsValid)
            {
                // 1. Save the announcement to the database (optional, but recommended for record-keeping)
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();

                var programTitle = await _context.Programs
                  .Where(p => p.ProgramId == announcement.ProgramId)
                  .Select(p => p.Title)
                  .FirstOrDefaultAsync();
                // 2. Get trainee emails for the program
                var traineeEmails = await _context.Enrollment
                    .Where(e => e.ProgramId == announcement.ProgramId && e.EnrollmentStatus == "Approved") // Filter by approved enrollments
                    .Include(e => e.CurrentTrainee)
                    .Select(e => e.CurrentTrainee.Email)
                    .ToListAsync();

                // 3. Send email
                if (traineeEmails.Any()) // Check if any trainees are enrolled
                {
                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential("serviceacademyedu@gmail.com", "kbwj agpd ptyb qgzm"); // Gmail credentials
                        smtpClient.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("your_gmail_email@gmail.com"), // Your Gmail email
                            Subject = $"Announcement from {programTitle}: {announcement.AnnouncementTitle}",
                            Body = announcement.Content,
                            IsBodyHtml = true // If you're sending HTML content
                        };

                        foreach (var email in traineeEmails)
                        {
                            mailMessage.To.Add(email);
                        }

                        try
                        {
                            await smtpClient.SendMailAsync(mailMessage);
                            TempData["ProgramStreamSuccessMessage"] = "Announcement sent successfully!"; // Success message
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending email");
                            TempData["ProgramStreamErrorMessage"] = "Error sending announcement. Please try again later."; // Error message
                        }
                    }
                }
                else
                {
                    TempData["InfoMessage"] = "No trainees enrolled in this program."; // Informational message
                }


                return RedirectToAction("ProgramStream", new { programId = announcement.ProgramId }); // Redirect back to ProgramStream
            }

            // ... (Handle ModelState errors if needed)
            TempData["ProgramStreamErrorMessage"] = "Error creating announcement. Please check the form and try again.";
            return RedirectToAction("ProgramStream", new { programId = announcement.ProgramId });
        }
        #endregion
    }
}


