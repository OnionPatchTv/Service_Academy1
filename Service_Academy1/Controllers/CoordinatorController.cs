using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAcademy.Controllers;
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
            _logger = logger;
            _context = context;
        }
        public IActionResult CoordinatorDashboard()
        {
            ViewData["ActivePage"] = "Dashboard";
            return View();
        }

        // Analytics action
        public IActionResult Analytics()
        {
            ViewData["ActivePage"] = "Analytics";
            return View();
        }
        // ManageProgram action: Handles the program management view
        public IActionResult ManageProgram()
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
                    p.Agenda
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

