using Microsoft.AspNetCore.Mvc;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;

namespace ServiceAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            (_logger, _context) = (logger, context);
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

        public IActionResult Faqs()
        {
            ViewData["ActivePage"] = "Faqs";
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
