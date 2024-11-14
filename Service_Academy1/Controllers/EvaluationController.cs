using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EvaluationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EvaluationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create(int programId)
    {
        var learnerId = _userManager.GetUserId(User);

        // Check enrollment and if the user is authorized to evaluate
        var enrollment = await _context.Enrollment
            .Include(x => x.ProgramsModel)
            .FirstOrDefaultAsync(e => e.ProgramId == programId && e.TraineeId == learnerId && e.EnrollmentStatus == "Approved");

        if (enrollment == null)
        {
            TempData["ErrorMessage"] = "You are not authorized to evaluate this program.";
            return RedirectToAction("TraineeDashboard", "Trainee", new { id = programId });
        }


        // Define standard questions categorized by different criteria
        var standardQuestions = new Dictionary<string, List<EvaluationQuestion>>
        {
            { "Performance", new List<EvaluationQuestion> {
                new EvaluationQuestion { QuestionId = 1, QuestionText = "Overall, how effectively did the program achieve its stated learning objectives?", Category = "Performance", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 2, QuestionText = "To what extent did the program content align with your expectations?", Category = "Performance", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 3, QuestionText = "How well did the program structure facilitate your learning process?", Category = "Performance", ProgramId = programId }
            }},
            { "Satisfaction", new List<EvaluationQuestion> {
                new EvaluationQuestion { QuestionId = 4, QuestionText = "Overall, how satisfied were you with the program?", Category = "Satisfaction", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 5, QuestionText = "How satisfied were you with the quality of instruction/materials?", Category = "Satisfaction", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 6, QuestionText = "How likely are you to recommend this program to others?", Category = "Satisfaction", ProgramId = programId }
            }},
            { "Quality", new List<EvaluationQuestion> {
                new EvaluationQuestion { QuestionId = 7, QuestionText = "How would you rate the clarity and organization of the program content?", Category = "Quality", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 8, QuestionText = "How would you rate the relevance and usefulness of the program content to your needs?", Category = "Quality", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 9, QuestionText = "How would you rate the overall quality of the learning experience?", Category = "Quality", ProgramId = programId }
            }},
            { "Continuity", new List<EvaluationQuestion> {
                new EvaluationQuestion { QuestionId = 10, QuestionText = "How likely are you to apply what you learned in this program in the future?", Category = "Continuity", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 11, QuestionText = "How helpful was this program in achieving your learning/professional goals?", Category = "Continuity", ProgramId = programId },
                new EvaluationQuestion { QuestionId = 12, QuestionText = "Would you be interested in participating in similar programs in the future?", Category = "Continuity", ProgramId = programId }
            }}
        }.SelectMany(x => x.Value).ToList();

        if (standardQuestions == null || !standardQuestions.Any())
        {
            TempData["ErrorMessage"] = "No evaluation questions available for this program.";
            return RedirectToAction("MyLearningStream", new { programId = programId });
        }

        var viewModel = new EvaluationFormViewModel
        {
            ProgramId = programId,
            Questions = standardQuestions,
            Responses = standardQuestions.Select(q => new ResponseViewModel { QuestionId = q.QuestionId }).ToList()
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

        // Check if the user has already evaluated the program
        bool hasEvaluated = await _context.EvaluationResponses
            .AnyAsync(er => er.LearnerId == learnerId && er.ProgramId == model.ProgramId);

        if (hasEvaluated)
        {
            return Json(new { success = false, message = "You have already evaluated this program." });
        }

        // Save each response
        foreach (var response in model.Responses)
        {
            var evaluationResponse = new EvaluationResponse
            {
                QuestionId = response.QuestionId,
                ProgramId = model.ProgramId,
                LearnerId = learnerId,
                Rating = response.Rating
            };
            _context.EvaluationResponses.Add(evaluationResponse);
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true, programId = model.ProgramId });
    }

    public async Task<IActionResult> EvaluationResults(int programId)
    {
        // Fetch average ratings grouped by category and include the related question text
        var averageRatings = await _context.EvaluationResponses
            .Include(er => er.EvaluationQuestion)
            .Where(r => r.ProgramId == programId)
            .GroupBy(r => r.EvaluationQuestion.Category)
            .Select(g => new ResultViewModel
            {
                Category = g.Key,
                AverageRating = g.Average(r => r.Rating),
                QuestionText = g.FirstOrDefault().EvaluationQuestion.QuestionText
            })
            .ToListAsync();

        // Retrieve the program details for displaying title, or set a default if not found
        var program = await _context.Programs.FindAsync(programId);

        // Count distinct learners who have evaluated
        var evaluatedCount = await _context.EvaluationResponses
            .Where(er => er.ProgramId == programId)
            .Select(er => er.LearnerId)
            .Distinct()
            .CountAsync();

        // Count total approved trainees for the program
        var totalTrainees = await _context.Enrollment
            .Where(e => e.ProgramId == programId && e.EnrollmentStatus == "Approved")
            .CountAsync();

        // Retrieve all questions for the specified program ID
        var questions = await _context.EvaluationQuestions
            .Where(x => x.ProgramId == programId)
            .ToListAsync();

        // Create the view model with all necessary data
        var viewModel = new EvaluationResultsViewModel
        {
            ProgramTitle = program?.Title ?? "Program Not Found",
            AverageRatings = averageRatings,
            EvaluatedCount = evaluatedCount,
            TotalTrainees = totalTrainees,
            UnevaluatedCount = totalTrainees - evaluatedCount,
            ProgramId = programId,
            Questions = questions // Set Questions property with the result from the query
        };

        return View(viewModel);
    }

    public async Task<IActionResult> MyEvaluations()
    {
        var learnerId = _userManager.GetUserId(User);

        var myEvaluations = await _context.EvaluationResponses
            .Include(er => er.EvaluationQuestion)
            .Include(er => er.EvaluationQuestion.ProgramsModel)
            .Where(er => er.LearnerId == learnerId)
            .ToListAsync();

        return View(myEvaluations);
    }
}

