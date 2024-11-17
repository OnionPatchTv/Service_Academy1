using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service_Academy1.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServiceAcademy.Controllers
{
    public class ProgramCreateController : Controller
    {
        private readonly ILogger<ProgramCreateController> _logger;
        private readonly ApplicationDbContext _context;

        public ProgramCreateController(ILogger<ProgramCreateController> logger, ApplicationDbContext context)
        {
            (_logger, _context) = (logger, context);
        }

        [HttpGet]
        public IActionResult ProgramCreation()
        {
            var projectleaderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var projectleaderName = User.FindFirstValue(ClaimTypes.Name);

            // Fetch the project leader's department
            var projectLeader = _context.Users.FirstOrDefault(u => u.Id == projectleaderId);
            var departmentName = string.Empty;

            if (projectLeader != null && projectLeader.DepartmentId.HasValue)
            {
                var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == projectLeader.DepartmentId.Value);
                departmentName = department?.DepartmentName ?? "Unknown Department";
            }

            var viewModel = new ProgramCreateViewModel
            {
                ProjectLeaderId = projectleaderId,
                ProjectLeader = projectleaderName,
                DepartmentName = departmentName,
                DepartmentId = projectLeader?.DepartmentId.ToString()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramCreation(ProgramsModel program, IFormFile photoInput, DateTime startDate, DateTime endDate)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(program);
            }

            if (photoInput != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CreatedImages");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + photoInput.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photoInput.CopyToAsync(fileStream);
                }

                program.PhotoPath = "/CreatedImages/" + uniqueFileName;
            }
            else
            {
                _logger.LogError("Photo input is required.");
                ModelState.AddModelError("photoInput", "The Photo field is required.");
                return View(program);
            }

            try
            {
                var projectleaderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var projectLeader = _context.Users.FirstOrDefault(u => u.Id == projectleaderId);

                if (projectLeader != null)
                {
                    program.ProjectLeaderId = projectleaderId;
                    program.ProjectLeader = User.FindFirstValue(ClaimTypes.Name);
                    program.DepartmentId = projectLeader.DepartmentId ?? 0; // Default to 0 if no department found
                }

                // Ensure SDG value is set from the view model
                program.SDG = program.SDG; // The SDG value is included as part of the ProgramsModel instance here.

                _context.Programs.Add(program);
                await _context.SaveChangesAsync();

                var programManagement = new ProgramManagementModel
                {
                    ProgramId = program.ProgramId,
                    StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                    IsArchived = false,
                    IsActive = false,
                    IsApproved = "Pending",
                    ReasonForDenial = null
                };

                _context.ProgramManagement.Add(programManagement);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Successfully added a Program";
                return RedirectToAction("ProgramCreation", "ProgramCreate");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving program to the database.");
                TempData["Error"] = "Failed to add a Program. Please try again.";
                return View(program);
            }
        }

    }
}
