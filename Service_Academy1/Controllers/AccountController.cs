using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace ServiceAcademy.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Registration View
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Wrap role assignment in a try-catch block 
                    try
                    {
                        var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Student");
                        if (!roleAssignmentResult.Succeeded)
                        {
                            // Log detailed errors from AddToRoleAsync
                            foreach (var error in roleAssignmentResult.Errors)
                            {
                                _logger.LogError("Error assigning 'Student' role: Code={ErrorCode}, Description={ErrorDescription}", error.Code, error.Description);
                            }

                            // Add an error to ModelState to signal an issue
                            ModelState.AddModelError(string.Empty, "An error occurred while assigning the role. Please try again later.");

                            // Re-render the registration form with the error
                            return View(model); // Or return a different error view if needed
                        }
                        else
                        {
                            // Role assignment succeeded, log the success
                            _logger.LogInformation("Successfully assigned 'Student' role to user: {UserName}", user.UserName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An exception occurred during role assignment: {ExceptionMessage}", ex.Message);
                        ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
                        return View(model);
                    }

                    // Proceed with login and redirection if role assignment was successful
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("MyLearning", "Trainee");
                }

                // Handle user creation errors (if result.Succeeded is false) 
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // Redisplay the registration form with any errors
            return View(model);
        }

        // Login View
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        // Debug output to see assigned roles
                        Debug.WriteLine($"Roles for user {user.Email}: {string.Join(", ", roles)}");

                        // Redirect to the appropriate page after login
                        if (roles.Contains("Student"))
                        {
                            return RedirectToAction("MyLearning", "Trainee");
                        }
                        else if (roles.Contains("ProjectLeader"))
                        {
                            return RedirectToAction("ProjectLeaderDashboard", "ProjectLeader");
                        }
                        else if (roles.Contains("Coordinator"))
                        {
                            return RedirectToAction("CoordDashboard", "Coordinator");
                        }
                        else if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Dashboard", "Admin");
                        }
                        // Default redirect if no specific role
                        return RedirectToAction("Home", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}