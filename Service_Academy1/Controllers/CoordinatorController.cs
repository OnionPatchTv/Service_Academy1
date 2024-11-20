using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAcademy.Controllers;
using System;
using System.Security.Claims;

namespace Service_Academy1.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ILogger<CoordinatorController> _logger;
        private readonly ApplicationDbContext _context;

        public CoordinatorController(ILogger<CoordinatorController> logger, ApplicationDbContext context)
        {
            (_logger, _context) = (logger, context);
        }
        public IActionResult CoordDashboard()
        {
            ViewData["ActivePage"] = "Dashboard";
            return View();
        }

        // Analytics action
        public async Task<IActionResult> CoordAnalytics()
        {
            ViewData["ActivePage"] = "Analytics";

            // Get the coordinator's department ID
            var coordinatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coordinator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == coordinatorId);

            if (coordinator == null || coordinator.DepartmentId == null)
            {
                return Unauthorized(); // Return if coordinator or department not found
            }

            var departmentId = coordinator.DepartmentId.Value;

            // Fetch programs for the coordinator's department
            var programs = await _context.Programs
                .Where(p => p.DepartmentId == departmentId)
                .ToListAsync();

            var programIds = programs.Select(p => p.ProgramId).ToList();

            // *** Existing Program Performance Chart Code ***
            var programEvaluationData = await _context.EvaluationResponses
                .Include(r => r.EvaluationQuestions)
                .Where(r => programIds.Contains(r.EvaluationQuestions.ProgramId))
                .GroupBy(r => r.EvaluationQuestions.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToListAsync();

            var topPrograms = programEvaluationData
                .OrderByDescending(p => p.AverageRating)
                .Take(3)
                .ToList();

            var programTitles = new List<string>();
            var averageRatings = new List<double>();

            foreach (var program in topPrograms)
            {
                var programTitle = await _context.Programs
                    .Where(p => p.ProgramId == program.ProgramId)
                    .Select(p => p.Title)
                    .FirstOrDefaultAsync();

                programTitles.Add(programTitle);
                averageRatings.Add(program.AverageRating);
            }

            // *** Program Completion Rate ***
            var completionRates = await _context.Enrollment
                .Where(e => programIds.Contains(e.ProgramId))
                .GroupBy(e => e.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    CompletionRate = g.Count(e => e.ProgramStatus == "Completed") * 100.0 / g.Count()
                })
                .ToListAsync();

            var completionTitles = new List<string>();
            var completionValues = new List<double>();

            foreach (var rate in completionRates)
            {
                var programTitle = await _context.Programs
                    .Where(p => p.ProgramId == rate.ProgramId)
                    .Select(p => p.Title)
                    .FirstOrDefaultAsync();

                completionTitles.Add(programTitle);
                completionValues.Add(rate.CompletionRate);
            }

            // *** Quiz Performance Analytics ***
            var quizPerformanceByProgram = await _context.TraineeQuizResults
                    .Include(r => r.Quiz)
                    .Where(r => programIds.Contains(r.Quiz.ProgramId)) // Filter by the coordinator's programs
                    .GroupBy(r => r.Quiz.ProgramId) // Group by ProgramId instead of QuizId
                    .Select(g => new
                    {
                        ProgramId = g.Key,
                        AverageScore = g.Average(r => r.ComputedScore), // Average score across all quizzes in the program
                        AverageRetries = g.Average(r => r.Retries)     // Average retries across all quizzes in the program
                    })
                    .ToListAsync();

            // Prepare data for the chart
            var quizProgramTitles = new List<string>();
            var programAverageScores = new List<double>();
            var programAverageRetries = new List<double>();

            foreach (var programData in quizPerformanceByProgram)
            {
                var programTitle = await _context.Programs
                    .Where(p => p.ProgramId == programData.ProgramId)
                    .Select(p => p.Title)
                    .FirstOrDefaultAsync();

                quizProgramTitles.Add(programTitle);
                programAverageScores.Add(programData.AverageScore);
                programAverageRetries.Add(programData.AverageRetries);
            }
            // Activity Completion Rate and Average Score Analytics
            var activityPerformanceByProgram = await _context.TraineeActivities
                .Include(r => r.Activities)
                .Where(r => programIds.Contains(r.Activities.ProgramId)) // Filter by the coordinator's programs
                .GroupBy(r => r.Activities.ProgramId) // Group by ProgramId
                .Select(g => new
                {
                    ProgramId = g.Key,
                    AverageScore = g.Average(r => r.ComputedScore), // Average score across all activities in the program
                    CompletionRate = g.Count(r => r.IsCompleted) * 100.0 / g.Count() // Completion rate
                })
                .ToListAsync();

            // Prepare data for the activity performance chart
            var activityProgramTitles = new List<string>();
            var programActivityAverageScores = new List<double>();
            var programActivityCompletionRates = new List<double>();

            foreach (var programData in activityPerformanceByProgram)
            {
                var programTitle = await _context.Programs
                    .Where(p => p.ProgramId == programData.ProgramId)
                    .Select(p => p.Title)
                    .FirstOrDefaultAsync();

                activityProgramTitles.Add(programTitle);
                programActivityAverageScores.Add(programData.AverageScore);
                programActivityCompletionRates.Add(programData.CompletionRate);
            }
            var overallProgramProgress = await _context.Programs
             .Where(p => programIds.Contains(p.ProgramId))
             .Select(p => new
             {
                 ProgramId = p.ProgramId,
                 ProgramTitle = p.Title,

                 // Module Progress (using DefaultIfEmpty to avoid nulls)
                 ModuleProgress = _context.TraineeModuleResults
                    .Where(m => m.Module.ProgramId == p.ProgramId)
                    .GroupBy(m => m.Module.ProgramId)
                    .Select(g => g.Average(m => m.IsCompleted ? 1 : 0))
                    .FirstOrDefault(),

                 // Quiz Progress (default to 0 for nulls)
                 QuizProgress = _context.TraineeQuizResults
                    .Where(q => q.Quiz.ProgramId == p.ProgramId)
                    .DefaultIfEmpty() // If there are no results, return 0 as the default value
                    .Average(q => q.ComputedScore == null ? 0 : q.ComputedScore),

                 // Activity Progress (default to 0 for nulls)
                 ActivityProgress = _context.TraineeActivities
                     .Where(a => a.Activities.ProgramId == p.ProgramId)
                     .DefaultIfEmpty() // If there are no results, return 0 as the default value
                     .Average(q => q.ComputedScore == null ? 0 : q.ComputedScore),
             })
             .ToListAsync();


            // Calculate the Overall Progress as an average of these components
            var programProgressTitles = new List<string>();
            var programOverallProgress = new List<double>();

            foreach (var programData in overallProgramProgress)
            {
                // Combine the different progress metrics for each program
                var overallProgress = (programData.ModuleProgress + programData.QuizProgress + programData.ActivityProgress) / 3;

                programProgressTitles.Add(programData.ProgramTitle);
                programOverallProgress.Add(overallProgress);
            }


            // Pass data to ViewBag
            ViewBag.ProgramTitles = programTitles;
            ViewBag.AverageRatings = averageRatings;
            ViewBag.CompletionTitles = completionTitles;
            ViewBag.CompletionValues = completionValues;
            ViewBag.QuizProgramTitles = quizProgramTitles;
            ViewBag.ProgramAverageScores = programAverageScores;
            ViewBag.ProgramAverageRetries = programAverageRetries;
            ViewBag.ActivityProgramTitles = activityProgramTitles;
            ViewBag.ProgramActivityAverageScores = programActivityAverageScores;
            ViewBag.ProgramActivityCompletionRates = programActivityCompletionRates;
            ViewBag.ProgramProgressTitles = programProgressTitles;
            ViewBag.ProgramOverallProgress = programOverallProgress;


            return View();
        }

        private string GetAgendaFullName(string agendaCode)
        {
            return agendaCode switch
            {
                "BISIG" => "BatStateU Inclusive Social Innovation for Regional Growth (BISIG)",
                "LEAF" => "Livelihood and Other Entrepreneurship related on Agri-Fisheries (LEAF)",
                "Environment" => "Environment and Natural Resources Conservation, Protection and Rehabilitation",
                "SAEI" => "Smart Analytics for Engineering Innovation",
                "BINADI" => "Adopt-A-Municipality / Social Development Through BINADI Implementation",
                "Outreach" => "Community Outreach",
                "TVET" => "Technical-Vocational Education And Training (TVET)",
                "TTAU" => "Technology Transfer, And Adoption / Utilization",
                "TAAS" => "Technical Assistance And Advisory Services",
                "PESODEV" => "Parents' Empowerment Thru Social Development",
                "GAD" => "Gender And Development",
                "DisasterRisk" => "Disaster Risk Reduction And Management And Disaster Preparedness And Response / Climate Change Adoption",
                _ => agendaCode // Return the acronym if no match found
            };
        }

        private string GetSdgFullName(string sdgCode)
        {
            return sdgCode switch
            {
                "NP" => "No Poverty",
                "ZH" => "Zero Hunger",
                "GHWB" => "Good Health and Well Being",
                "QE" => "Quality Education",
                "GE" => "Gender Equality",
                "CWS" => "Clean Water and Sanitation",
                "ACE" => "Affordable and Clean Energy",
                "DWEG" => "Decent Work and Economic Growth",
                "III" => "Industry, Innovation and Infrastructure",
                "RI" => "Reduced Inequalities",
                "SCC" => "Sustainable Cities and Communities",
                "RCP" => "Responsible Consumption and Production",
                "CA" => "Climate Action",
                "LBW" => "Life Below Water",
                "LL" => "Life on Land",
                "PJSI" => "Peace, Justice and Strong Institutions",
                "PG" => "Partnerships for the Goals",
                _ => sdgCode // Return the acronym if no match found
            };
        }
        // ManageProgram action: Handles the program management view
        public IActionResult CoordManageProgram()
        {
            // Get the coordinator's department ID
            var coordinatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coordinator = _context.Users.FirstOrDefault(u => u.Id == coordinatorId);

            if (coordinator == null || coordinator.DepartmentId == null)
            {
                return Unauthorized(); // Return if coordinator or department not found
            }

            var departmentId = coordinator.DepartmentId.Value;

            // Fetch programs for the coordinator's department
            var programs = _context.Programs
                .Include(p => p.ProgramManagement)
                .Where(p => p.DepartmentId == departmentId)
                .ToList();

            // Add full names for Agenda and SDG fields
            foreach (var program in programs)
            {
                program.Agenda = GetAgendaFullName(program.Agenda);
                program.SDG = GetSdgFullName(program.SDG);
            }

            ViewData["ActivePage"] = "ManageProgram";
            return View(programs); // Pass the filtered list of programs to the view
        }
        [HttpGet]
        public async Task<IActionResult> GetProgramDetails(int programId)
        {
            var coordinatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coordinator = _context.Users.FirstOrDefault(u => u.Id == coordinatorId);

            if (coordinator == null || coordinator.DepartmentId == null)
            {
                return Unauthorized(); // Return if coordinator or department not found
            }

            var program = await _context.Programs
                .Where(p => p.ProgramId == programId && p.DepartmentId == coordinator.DepartmentId)
                .Select(p => new
                {
                    p.Description,
                    p.Agenda,
                    p.SDG
                })
                .FirstOrDefaultAsync();

            if (program == null)
            {
                return Json(new { error = "Program not found" });
            }

            return Json(program);
        }


        // Approve Program Action
        [HttpPost]
        public async Task<IActionResult> ApproveProgram(int programId)
        {
            var programManagement = await _context.ProgramManagement
                .FirstOrDefaultAsync(pm => pm.ProgramId == programId);

            if (programManagement == null)
            {
                return NotFound();
            }

            // Change the approval status to "Approved"
            programManagement.IsApproved = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction("CoordManageProgram");
        }

        // Deny Program Action
        [HttpPost]
        public async Task<IActionResult> DenyProgram(int programId, string reasonForDenial)
        {
            var programManagement = await _context.ProgramManagement
                .FirstOrDefaultAsync(pm => pm.ProgramId == programId);

            if (programManagement == null)
            {
                return NotFound();
            }

            // Change the approval status to "Denied"
            programManagement.IsApproved = "Denied";
            // Store the reason for denial (if needed)
            programManagement.ReasonForDenial = reasonForDenial;
            await _context.SaveChangesAsync();

            return RedirectToAction("CoordManageProgram");
        }
    }
}

