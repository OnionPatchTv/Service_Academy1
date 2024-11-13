using Microsoft.AspNetCore.Mvc;
using Service_Academy1.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using iText.Commons.Actions.Contexts;

namespace ServiceAcademy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        // Dashboard action
        public IActionResult Dashboard()
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

        // ManageAccount action: Get users and display in the view
        public async Task<IActionResult> ManageAccount()
        {
            ViewData["ActivePage"] = "ManageAccount";

            // Fetch the list of users
            var model = new ManageAccountViewModel
            {
                Users = await _userManager.Users.ToListAsync()
            };

            return View(model);
        }

        // CreateAccount action: Handles the creation of new users
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FullName = model.Username // Assuming FullName is the same as Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
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

            // Fetch the list of users to be passed into the view, along with the CreateAccount form data
            var manageAccountViewModel = new ManageAccountViewModel
            {
                Users = await _userManager.Users.ToListAsync(),
                CreateAccountForm = model // Include the form data so it can be displayed again
            };

            // Return the ManageAccount view with both the user list and the form data
            return View("ManageAccount", manageAccountViewModel);
        }

        // EditAccount action: Handles editing of existing users
        [HttpPost]
        public async Task<IActionResult> EditAccount(EditAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user != null)
                {
                    user.Email = model.Email;
                    user.FullName = model.FullName;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Update user roles if necessary
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.Role);

                        TempData["SuccessMessage"] = "Account updated successfully!";
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return NotFound(); // Return NotFound() if the user doesn't exist
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update the account."; // Set error message for validation errors
            }

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

            ViewData["ActivePage"] = "ManageProgram";
            return View(programs); // Pass the list of programs to the view
        }
        [HttpGet]
        public async Task<IActionResult> GetProgramDetails(int programId)
        {
            var program = await _context.Programs
                .Where(p => p.ProgramId == programId)
                .Select(p => new
                {
                    p.Description,
                    p.Agenda
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
