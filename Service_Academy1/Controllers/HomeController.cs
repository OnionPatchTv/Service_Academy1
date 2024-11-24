using Microsoft.AspNetCore.Mvc;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using Service_Academy1.Services;

namespace ServiceAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, EmailService emailService)
        {
            (_logger, _context, _emailService) = (logger, context, emailService);
        }

        // Action method for Home.cshtml
        public IActionResult Home()
        {
            ViewData["ActivePage"] = "Home";
            return View();
        }


        public IActionResult ProgramCatalog()
        {
            ViewData["ActivePage"] = "ProgramCatalog";

            // Get all programs from the database (including ProgramManagement)
            var programs = _context.Programs
                .Include(p => p.ProgramManagement)
                .ToList();

            // Group programs by agenda
            var programsByAgenda = programs.GroupBy(p => p.Agenda).ToList();

            // Calculate counts for each agenda
            foreach (var agendaGroup in programsByAgenda)
            {
                // Active Programs
                var activeCount = agendaGroup.Where(p => p.ProgramManagement.Any(pm => pm.IsActive && pm.IsApproved == "Approved" && !pm.IsArchived))
                                            .Count();

                // Deactivated Programs
                var deactivatedCount = agendaGroup.Where(p => p.ProgramManagement.Any(pm => !pm.IsActive && pm.IsApproved == "Approved" && !pm.IsArchived))
                                                 .Count();

                // Set counts in ViewData (with default values)
                ViewData[$"Active_{agendaGroup.Key}"] = activeCount;
                ViewData[$"Deactivated_{agendaGroup.Key}"] = deactivatedCount;
            }

            // Handle cases where no programs exist for a specific agenda
            foreach (var agenda in new[] { "BISIG", "LEAF", "Environment", "SAEI", "BINADI", "Outreach", "TVET", "TTAU", "TAAS", "PESODEV", "GAD", "DisasterRisk" })
            {
                if (!programsByAgenda.Any(g => g.Key == agenda))
                {
                    ViewData[$"Active_{agenda}"] = 0;
                    ViewData[$"Deactivated_{agenda}"] = 0;
                }
            }

            return View(programs);
        }

        public IActionResult Contact()
        {
            ViewData["ActivePage"] = "Contact";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string name, string email, string message)
        {
            // Validate the form data (optional)
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                return Content("Please fill in all fields.");
            }

            try
            {
                // Send email via the email service
                await _emailService.SendEmailAsync(
                    toEmail: "serviceacademyedu@gmail.com",  // The email address where the form is sent
                    subject: $"Contact Us - {name}",  // Subject including the sender's name
                    body: $"<h1>Message from {name}</h1><p>{message}</p><p>Reply to: {email}</p>", // Email body
                    replyToEmail: email // Set the Reply-To header to the user's email address
                );

                // Provide success message to the user
                return Content("Your message has been sent successfully. We'll get back to you shortly.");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during email sending
                return Content($"Error: {ex.Message}");
            }
        }
        public IActionResult Faqs()
        {
            ViewData["ActivePage"] = "Faqs";
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult VerifyCertificate()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyCertificates(string certificateId)
        {
            try
            {
                var certificate = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.CertificateHash == certificateId);

                if (certificate == null)
                {
                    return Json(new { isValid = false, message = "Certificate not found or invalid." });
                }

                return Json(new { isValid = true, message = "Certificate is authentic." });
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }

    public class CertificateVerificationRequest
    {
        public string CertificateId { get; set; }
    }
}


