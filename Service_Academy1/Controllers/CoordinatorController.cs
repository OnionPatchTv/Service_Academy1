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
        private readonly ArliAIService _arliAIService;


        public CoordinatorController(ILogger<CoordinatorController> logger, ApplicationDbContext context, ArliAIService arliAIService)
        {
            (_logger, _context, _arliAIService) = (logger, context, arliAIService);
        }
        public IActionResult CoordDashboard()
        {
            ViewData["ActivePage"] = "Dashboard";
            return View();
        }

        // Analytics action
        public async Task<IActionResult> CoordAnalyticsDashboard()
        {
            ViewData["ActivePage"] = "Analytics Dashboard";

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

            // *** Existing Program Performance Code ***
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
                .Take(5)
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

            // *** Program Completion Rates ***
            var completionRates = await _context.Enrollment
                .Where(e => programIds.Contains(e.ProgramId))
                .GroupBy(e => e.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    CompletionRate = g.Count(e => e.ProgramStatus == "Complete") * 100.0 / g.Count()
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
                .Where(r => programIds.Contains(r.Quiz.ProgramId))
                .GroupBy(r => r.Quiz.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    AverageScore = g.Average(r => r.ComputedScore),
                    AverageRetries = g.Average(r => r.Retries)
                })
                .ToListAsync();

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

            // *** Activity Performance Analytics ***
            var activityPerformanceByProgram = await _context.TraineeActivities
                .Include(r => r.Activities)
                .Where(r => programIds.Contains(r.Activities.ProgramId))
                .GroupBy(r => r.Activities.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    CompletionRate = g.Count(r => r.IsCompleted) * 100.0 / g.Count(),
                    AverageScore = g.Average(r => r.ComputedScore)
                })
                .ToListAsync();

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

            // *** Overall Program Progress ***
            var overallProgramProgress = await _context.Programs
                .Where(p => programIds.Contains(p.ProgramId))
                .Select(p => new
                {
                    ProgramId = p.ProgramId,
                    ProgramTitle = p.Title,
                    ModuleProgress = _context.TraineeModuleResults
                        .Where(m => m.Module.ProgramId == p.ProgramId)
                        .GroupBy(m => m.Module.ProgramId)
                        .Select(g => g.Average(m => m.IsCompleted ? 1 : 0))
                        .FirstOrDefault(),
                    QuizProgress = _context.TraineeQuizResults
                        .Where(q => q.Quiz.ProgramId == p.ProgramId)
                        .DefaultIfEmpty()
                        .Average(q => q.ComputedScore == null ? 0 : q.ComputedScore),
                    ActivityProgress = _context.TraineeActivities
                        .Where(a => a.Activities.ProgramId == p.ProgramId)
                        .DefaultIfEmpty()
                        .Average(q => q.ComputedScore == null ? 0 : q.ComputedScore),
                })
                .ToListAsync();

            var programProgressTitles = new List<string>();
            var programOverallProgress = new List<double>();

            foreach (var programData in overallProgramProgress)
            {
                var overallProgress = (programData.ModuleProgress + programData.QuizProgress + programData.ActivityProgress) / 3;
                programProgressTitles.Add(programData.ProgramTitle);
                programOverallProgress.Add(overallProgress);
            }
            var systemUsageData = await _context.SystemUsageLogs
               .GroupBy(log => new { log.ActionType, log.Timestamp.Date })
               .Select(g => new
               {
                   ActionType = g.Key.ActionType,
                   Date = g.Key.Date,
                   Count = g.Count()
               })
               .ToListAsync();

            // Organize data for each ActionType
            var loginData = systemUsageData.Where(d => d.ActionType == "Login").ToList();
            var quizSubmissionData = systemUsageData.Where(d => d.ActionType == "QuizSubmission").ToList();
            var activitySubmissionData = systemUsageData.Where(d => d.ActionType == "ActivitySubmission").ToList();
            var programEnrollmentData = systemUsageData.Where(d => d.ActionType == "ProgramEnrollment").ToList();

            // Get unique dates sorted by date
            var dates = systemUsageData.Select(d => d.Date.ToString("yyyy-MM-dd")).Distinct().OrderBy(d => d).ToList();

            // Prepare counts for each action type
            var loginCounts = dates.Select(date => loginData.Where(d => d.Date.ToString("yyyy-MM-dd") == date).Sum(d => d.Count)).ToList();
            var quizSubmissionCounts = dates.Select(date => quizSubmissionData.Where(d => d.Date.ToString("yyyy-MM-dd") == date).Sum(d => d.Count)).ToList();
            var activitySubmissionCounts = dates.Select(date => activitySubmissionData.Where(d => d.Date.ToString("yyyy-MM-dd") == date).Sum(d => d.Count)).ToList();
            var programEnrollmentCounts = dates.Select(date => programEnrollmentData.Where(d => d.Date.ToString("yyyy-MM-dd") == date).Sum(d => d.Count)).ToList();


            // Pass data to ViewBag for displaying charts and other analytics
            ViewBag.Recommendation = TempData["Recommendation"] as string;
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
            ViewBag.Dates = dates;
            ViewBag.LoginCounts = loginCounts;
            ViewBag.QuizSubmissionCounts = quizSubmissionCounts;
            ViewBag.ActivitySubmissionCounts = activitySubmissionCounts;
            ViewBag.ProgramEnrollmentCounts = programEnrollmentCounts;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenerateRecommendation()
        {
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

            var programTitlesById = programs.ToDictionary(p => p.ProgramId, p => p.Title);

            var programIds = programs.Select(p => p.ProgramId).ToList();

            if (!programIds.Any())
            {
                // No programs found for the department
                TempData["Recommendation"] = "No data available to generate a recommendation for this department.";
                return RedirectToAction("CoordAnalyticsDashboard");
            }

            // Collect analytics data
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

            var completionRates = await _context.Enrollment
                .Where(e => programIds.Contains(e.ProgramId))
                .GroupBy(e => e.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    CompletionRate = g.Count(e => e.ProgramStatus == "Complete") * 100.0 / g.Count()
                })
                .ToListAsync();

            var quizPerformanceByProgram = await _context.TraineeQuizResults
                .Include(r => r.Quiz)
                .Where(r => programIds.Contains(r.Quiz.ProgramId))
                .GroupBy(r => r.Quiz.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    AverageScore = g.Average(r => r.ComputedScore),
                    AverageRetries = g.Average(r => r.Retries)
                })
                .ToListAsync();

            var activityPerformanceByProgram = await _context.TraineeActivities
                .Include(r => r.Activities)
                .Where(r => programIds.Contains(r.Activities.ProgramId))
                .GroupBy(r => r.Activities.ProgramId)
                .Select(g => new
                {
                    ProgramId = g.Key,
                    CompletionRate = g.Count(r => r.IsCompleted) * 100.0 / g.Count(),
                    AverageScore = g.Average(r => r.ComputedScore)
                })
                .ToListAsync();

            // Ensure there is data to work with
            if (!programEvaluationData.Any() && !completionRates.Any() && !quizPerformanceByProgram.Any() && !activityPerformanceByProgram.Any())
            {
                // Insufficient data for analysis
                TempData["Recommendation"] = "Insufficient data to generate meaningful recommendations for this department.";
                return RedirectToAction("CoordAnalyticsDashboard");
            }

            // Build the prompt dynamically based on available data
            var prompt = $@"
            Analyze the following dataset which includes program evaluation data, completion rates, quiz and activity performance, and overall progress metrics.
            The top programs based on average ratings are: {string.Join(", ", programEvaluationData.OrderByDescending(p => p.AverageRating).Take(5).Select(p => programTitlesById[p.ProgramId]))}.
            Program completion rates by program are as follows: {string.Join(", ", completionRates.Select(c => $"{programTitlesById[c.ProgramId]}: {c.CompletionRate}%"))}.
            Quiz performance (average score and retries) is as follows: {string.Join(", ", quizPerformanceByProgram.Select(q => $"{programTitlesById[q.ProgramId]}: Avg Score {q.AverageScore}, Avg Retries {q.AverageRetries}"))}.
            Activity performance (completion rates and average score) is as follows: {string.Join(", ", activityPerformanceByProgram.Select(a => $"{programTitlesById[a.ProgramId]}: Completion {a.CompletionRate}%, Avg Score {a.AverageScore}"))}.

            Consider this data and provide a clear, concise 3-8 sentence insight into the overall effectiveness and areas for improvement in program delivery and engagement, with actionable recommendations for optimizing trainee outcomes and enhancing program performance. Avoid using lists or bullet points; write in a cohesive essay style.";
            // Get the recommendation from ArliAI
            var recommendation = await _arliAIService.GetRecommendation(prompt);

            // Store the recommendation in TempData for the next request
            TempData["Recommendation"] = recommendation;

            // Redirect back to the analytics dashboard
            return RedirectToAction("CoordAnalyticsDashboard");
        }


        private static string GetAgendaFullName(string agendaCode)
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

        private static string GetSdgFullName(string sdgCode)
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

