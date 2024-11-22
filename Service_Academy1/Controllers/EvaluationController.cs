using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service_Academy1.Controllers;
using Service_Academy1.Models;
using System;

namespace Service_Academy1.Controllers
{
    public class EvaluationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ArliAIService _arliAIService;

        public EvaluationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ArliAIService arliAIService)
        {
            (_userManager, _context, _arliAIService) = (userManager, context, arliAIService);

        }

        public async Task<IActionResult> EvaluationForm(int programId)
        {
            var learnerId = _userManager.GetUserId(User);

            // Check if user is enrolled and authorized to evaluate
            var enrollment = await _context.Enrollment
                .Include(x => x.ProgramsModel)
                .FirstOrDefaultAsync(e => e.ProgramId == programId && e.TraineeId == learnerId && e.EnrollmentStatus == "Approved");

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "You are not authorized to evaluate this program.";
                return RedirectToAction("TraineeDashboard", "Trainee", new { id = programId });
            }

            // Check if questions exist for this programId, if not, create them.
            var existingQuestions = await _context.EvaluationQuestions.Where(q => q.ProgramId == programId).ToListAsync();
            if (!existingQuestions.Any())
            {
                // Define standard questions
                var standardQuestions = new Dictionary<string, List<EvaluationQuestionModel>>
                {
                    { "Performance", new List<EvaluationQuestionModel> {
                        new EvaluationQuestionModel { QuestionText = "Overall, how effectively did the program achieve its stated learning objectives?", Category = "Performance" },
                        new EvaluationQuestionModel { QuestionText = "To what extent did the program content align with your expectations?", Category = "Performance" },
                        new EvaluationQuestionModel { QuestionText = "How well did the program structure facilitate your learning process?", Category = "Performance" }
                    }},
                    { "Satisfaction", new List<EvaluationQuestionModel> {
                        new EvaluationQuestionModel { QuestionText = "Overall, how satisfied were you with the program?", Category = "Satisfaction" },
                        new EvaluationQuestionModel { QuestionText = "How satisfied were you with the quality of instruction/materials?", Category = "Satisfaction" },
                        new EvaluationQuestionModel { QuestionText = "How likely are you to recommend this program to others?", Category = "Satisfaction" }
                    }},
                    { "Quality", new List<EvaluationQuestionModel> {
                        new EvaluationQuestionModel { QuestionText = "How would you rate the clarity and organization of the program content?", Category = "Quality" },
                        new EvaluationQuestionModel { QuestionText = "How would you rate the relevance and usefulness of the program content to your needs?", Category = "Quality" },
                        new EvaluationQuestionModel { QuestionText = "How would you rate the overall quality of the learning experience?", Category = "Quality" }
                    }},
                    { "Continuity", new List<EvaluationQuestionModel> {
                        new EvaluationQuestionModel { QuestionText = "How likely are you to apply what you learned in this program in the future?", Category = "Continuity" },
                        new EvaluationQuestionModel { QuestionText = "How helpful was this program in achieving your learning/professional goals?", Category = "Continuity" },
                        new EvaluationQuestionModel { QuestionText = "Would you be interested in participating in similar programs in the future?", Category = "Continuity" }
                    }}
                };

                // Add questions to database
                foreach (var categoryGroup in standardQuestions)
                {
                    foreach (var question in categoryGroup.Value)
                    {
                        question.ProgramId = programId; // Assign ProgramId to each question
                        _context.EvaluationQuestions.Add(question);
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Now fetch questions again (they should exist)
            var questions = await _context.EvaluationQuestions.Where(q => q.ProgramId == programId).ToListAsync();

            if (!questions.Any())
            {
                TempData["ErrorMessage"] = "No evaluation questions available for this program.";
                return RedirectToAction("MyLearningStream", new { programId = programId });
            }

            var viewModel = new EvaluationFormViewModel
            {
                ProgramId = programId,
                Questions = questions,
                Responses = questions.Select(q => new ResponseViewModel { QuestionId = q.EvaluationQuestionId }).ToList()
            };

            ViewBag.ProgramTitle = enrollment.ProgramsModel.Title;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(EvaluationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid form submission." });
            }

            var learnerId = _userManager.GetUserId(User);

            // Get the corresponding enrollment for this user and program
            var enrollment = await _context.Enrollment
                .FirstOrDefaultAsync(e => e.ProgramId == model.ProgramId && e.TraineeId == learnerId && e.EnrollmentStatus == "Approved");

            if (enrollment == null)
            {
                return Json(new { success = false, message = "Enrollment not found." });
            }

            // Check if the user has already evaluated the program
            bool hasEvaluated = await _context.EvaluationResponses
                .AnyAsync(er => er.EnrollmentId == enrollment.EnrollmentId && er.EvaluationQuestions.ProgramId == model.ProgramId);

            if (hasEvaluated)
            {
                return Json(new { success = false, message = "You have already evaluated this program." });
            }

            // Save each response
            foreach (var response in model.Responses)
            {
                var evaluationResponse = new EvaluationResponseModel
                {
                    EvaluationQuestionId = response.QuestionId,
                    EnrollmentId = enrollment.EnrollmentId,
                    Rating = response.Rating
                };
                _context.EvaluationResponses.Add(evaluationResponse);
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Successfully Answered the Evaluation Form!";
            // Redirect back to MyLearningStream.cshtml after successful submission
            return RedirectToAction("MyLearningStream", "Trainee", new { programId = model.ProgramId });
        }

        public async Task<IActionResult> EvaluationResults(int programId)
        {
            // Ensure programId is passed via the URL
            if (programId == 0)
            {
                return RedirectToAction("Error");
            }

            // Fetch evaluation response data, grouped by category and rating
            var evaluationResults = await _context.EvaluationResponses
                .Include(r => r.EvaluationQuestions)
                .Where(r => r.EvaluationQuestions.ProgramId == programId)
                .GroupBy(r => new { r.EvaluationQuestions.Category, r.Rating })
                .Select(g => new EvaluationResponseDetail
                {
                    Category = g.Key.Category,
                    Rating = g.Key.Rating,
                    Count = g.Count()
                })
                .ToListAsync();

            // Get the total number of approved trainees for the program
            var totalTrainees = await _context.Enrollment
                .Where(e => e.ProgramId == programId && e.EnrollmentStatus == "Approved")
                .CountAsync();

            // Count how many distinct trainees have submitted evaluations (based on EnrollmentId)
            var evaluatedCount = await _context.EvaluationResponses
                .Where(r => r.EvaluationQuestions.ProgramId == programId)
                .Select(r => r.EnrollmentId)
                .Distinct()
                .CountAsync();

            // The number of unevaluated trainees
            var unevaluatedCount = totalTrainees - evaluatedCount;

            // Calculate average ratings by category (corrected to avoid multiple counts)
            var averageRatings = evaluationResults
                .GroupBy(r => r.Category)
                .Select(g => new AverageRatingViewModel
                {
                    Category = g.Key,
                    AverageRating = g.Average(r => r.Rating)
                })
                .ToList();

            // Prepare the view model
            var viewModel = new EvaluationResultsViewModel
            {
                ProgramTitle = (await _context.Programs.FindAsync(programId))?.Title,
                TotalTrainees = totalTrainees,
                EvaluatedCount = evaluatedCount,
                UnevaluatedCount = unevaluatedCount,
                ProgramId = programId,
                AverageRatings = averageRatings,
                EvaluationDetails = evaluationResults
            };

            // If no evaluation results or no evaluated trainees, skip ArliAI analysis
            if (evaluationResults.Any() && evaluatedCount > 0)
            {
                // Prepare the first dynamic prompt for ArliAI
                var analysisPrompt = $@"
            Analyze the following dataset from a program's evaluation results, including evaluation responses categorized by rating, 
            total trainees, evaluated and unevaluated counts, and average ratings across categories. Consider the distribution of 
            evaluation responses, the proportion of evaluated versus unevaluated trainees, and the calculated average ratings for 
            each category. Provide a 3-5 sentence of cohesive analysis of the program's evaluation engagement and performance, offering actionable 
            insights to improve response rates, address gaps in trainee feedback, and enhance overall program effectiveness.
            Avoid using lists or bullet points; write in a cohesive essay style.";

                // Call ArliAI service to generate the first analysis
                var evaluationAnalysis = await _arliAIService.GetAnalysis(analysisPrompt);

                // Prepare the impact assessment prompt based on the first analysis
                var impactPrompt = $@"
            Based on the following analysis insights:
            {evaluationAnalysis}

            Assess the potential impact these recommendations could have on the program's overall quality, trainee satisfaction, and 
            program engagement. Include a 3-5 sentence analysis of how these changes might influence future evaluations, retention rates, and 
            stakeholder confidence. Provide a 3 sentence actionable next steps to ensure effective implementation of these recommendations.
            Avoid using lists or bullet points; write in a cohesive essay style.";

                // Call ArliAI service to generate the impact assessment
                var impactAssessment = await _arliAIService.GetImpact(impactPrompt);

                // Store the evaluation analysis and impact assessment in ViewBag for the view
                ViewBag.Analysis = evaluationAnalysis;
                ViewBag.ImpactAssessment = impactAssessment;
            }
            else
            {
                // If no results or no evaluated trainees, provide a message
                ViewBag.Analysis = "No evaluation data available to analyze.";
                ViewBag.ImpactAssessment = "No impact assessment can be generated due to lack of evaluation responses.";
            }

            // Return the view with the populated view model
            return View(viewModel);
        }

    }
}