using Microsoft.AspNetCore.Mvc;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using iText.Commons.Actions.Contexts;
using System.Security.Claims;

namespace ServiceAcademy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ArliAIService _arliAIService;

        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, ArliAIService arliAIService)
        {
            (_logger, _context, _arliAIService, _userManager) = (logger, context, arliAIService, userManager);
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
        // Analytics action
        public async Task<IActionResult> AnalyticsDashboard()
        {
            ViewData["ActivePage"] = "Analytics";

            // Check if the user is an admin
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole != "Admin")
            {
                return Unauthorized(); // Ensure only admin has access
            }

            // Fetch all programs (no need for departmentId)
            var programs = await _context.Programs.ToListAsync(); // Get all programs from all departments

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

            // *** Program Completion Rate ***
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
                    .Where(r => programIds.Contains(r.Quiz.ProgramId)) // Filter by all programs
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
                .Where(r => programIds.Contains(r.Activities.ProgramId)) // Filter by all programs
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

            // Overall Program Progress (similar to existing logic)
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

            // Calculate the Overall Progress as an average of these components
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

            // Pass data to ViewBag
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
            // Fetch data grouped by department
            var departmentData = await _context.Programs
                .GroupBy(p => p.DepartmentId)
                .Select(g => new
                {
                    DepartmentId = g.Key,
                    DepartmentName = _context.Departments
                        .Where(d => d.DepartmentId == g.Key)
                        .Select(d => d.DepartmentName)
                        .FirstOrDefault(),
                    AverageProgramRating = _context.EvaluationResponses
                        .Where(r => g.Select(p => p.ProgramId).Contains(r.EvaluationQuestions.ProgramId))
                        .Average(r => (double?)r.Rating) ?? 0,
                    CompletionRate = _context.Enrollment
                        .Where(e => g.Select(p => p.ProgramId).Contains(e.ProgramId))
                        .GroupBy(e => e.ProgramId)
                        .Select(gr => new { Rate = gr.Count(e => e.ProgramStatus == "Complete") * 100.0 / gr.Count() })
                        .Average(r => (double?)r.Rate) ?? 0,
                    OverallProgress = _context.TraineeModuleResults
                        .Where(m => g.Select(p => p.ProgramId).Contains(m.Module.ProgramId))
                        .Average(m => m.IsCompleted ? 1 : 0)
                })
                .ToListAsync();

            // Check if there's any data to recommend
            if (!departmentData.Any())
            {
                TempData["Recommendation"] = "No data available to generate recommendations at this time.";
                return RedirectToAction("Analytics");
            }

            // Prepare the prompt dynamically with detailed department-level insights
            var departmentInsights = string.Join("\n", departmentData.Select(d =>
                $"- {d.DepartmentName}: Average Rating: {d.AverageProgramRating:F2}, " +
                $"Completion Rate: {d.CompletionRate:F2}%, Overall Progress: {d.OverallProgress:F2}."
            ));

            var prompt = $@"Analyze the following dataset which includes program evaluation data, completion rates, quiz and activity performance, and overall progress metrics. 
                The departments are as follows: {departmentInsights}. 
                Consider this data and provide a clear, concise 3-8 sentence insight into the overall effectiveness and areas for improvement in program delivery and engagement at the departmental level, with actionable recommendations for optimizing trainee outcomes and enhancing program performance. Avoid using lists or bullet points; write in a cohesive essay style.";

            // Get recommendation from ArliAI
            var recommendation = await _arliAIService.GetRecommendation(prompt);

            // Store the recommendation in TempData
            TempData["Recommendation"] = recommendation;

            // Redirect to Analytics view
            return RedirectToAction("AnalyticsDashboard");
        }


        // ManageAccount action: Get users and display in the view
        public async Task<IActionResult> ManageAccount()
        {
            ViewData["ActivePage"] = "ManageAccount";

            // Fetch ALL users first (asynchronously)
            var allUsers = await _userManager.Users.ToListAsync();

            // Now filter synchronously using LINQ
            var filteredUsers = allUsers.Where(u => !_userManager.IsInRoleAsync(u, "Student").Result).ToList();


            var model = new ManageAccountViewModel { Users = filteredUsers };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Ensure Username is set to Email
                model.Username = model.Email;

                // Map department to DepartmentId
                int departmentId = int.Parse(model.Department ?? "0");

                // Create new ApplicationUser instance
                var user = new ApplicationUser
                {
                    UserName = model.Username,  // Automatically set Username from Email
                    Email = model.Email,
                    FullName = model.FullName,
                    DepartmentId = departmentId,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, model.Role);

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        await _userManager.DeleteAsync(user);
                        return View(model);
                    }

                    TempData["SuccessMessage"] = "Account created successfully!";
                    return RedirectToAction("ManageAccount");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            var manageAccountViewModel = new ManageAccountViewModel
            {
                Users = await _userManager.Users.ToListAsync(),
                CreateAccountForm = model
            };

            return View("ManageAccount", manageAccountViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditAccount(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    // Update user fields
                    user.UserName = model.Email;  // Ensure Username matches Email
                    user.Email = model.Email;
                    user.FullName = model.FullName;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Update user roles
                        var currentRoles = await _userManager.GetRolesAsync(user);

                        // Remove existing roles
                        if (currentRoles.Any())
                        {
                            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                            if (!removeResult.Succeeded)
                            {
                                foreach (var error in removeResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                                return View(model);
                            }
                        }

                        // Add the new role
                        var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
                        if (!addRoleResult.Succeeded)
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }

                        TempData["SuccessMessage"] = "Account updated successfully!";
                        return RedirectToAction("ManageAccount");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return NotFound();
                }
            }

            TempData["ErrorMessage"] = "Failed to update the account.";
            return RedirectToAction("ManageAccount");
        }

        // DeleteAccount action: Handles deletion of users
        [HttpPost] // Or [HttpDelete] for RESTful design
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Account deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the account.";
            }
            return RedirectToAction("ManageAccount");
        }

        // ManageProgram action: Handles the program management view
        public IActionResult ManageProgram()
        {
            // Assuming you have a context or repository to access the database
            var programs = _context.Programs
                .Include(p => p.ProgramManagement) // Load related ProgramManagement data
                .ToList();

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
            var program = await _context.Programs
                .Where(p => p.ProgramId == programId)
                .Select(p => new
                {
                    p.Description,
                    p.Agenda,
                    p.SDG
                })
                .FirstOrDefaultAsync();

            if (program == null)
            {
                return Json(null);
            }

            return Json(program); // Return the program details as JSON
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

            return RedirectToAction("ManageProgram");
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

            return RedirectToAction("ManageProgram");
        }

    }
}
