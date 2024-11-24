using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Service_Academy1.Services;
using iText.Commons.Actions.Contexts;
using Service_Academy1.Models;


namespace ServiceAcademy.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LogSystemUsageService _logUsageService;
        private readonly EmailService _emailService;
        private const int MaxFailedAttempts = 3;  // Maximum number of allowed failed attempts
        private readonly string FailedAttemptsSessionKey = "FailedAttempts";  // Session key for tracking failed attempts

        public AccountController(ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, LogSystemUsageService logUsageService, EmailService emailService)
        {
            (_logger, _context, _userManager, _signInManager, _logUsageService, _emailService) = (logger, context, userManager, signInManager, logUsageService, emailService);
        }

        // Registration View
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            if (!model.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Only Gmail addresses are allowed.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role ?? "Trainee");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    _logger.LogError($"Role assignment error: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await _userManager.DeleteAsync(user); // Rollback user creation
                return View(model);
            }

            // Save demographics
            var demographics = new UserDemographicsModel
            {
                ApplicationUserId = user.Id,
                DateOfBirth = DateTime.UtcNow,
                Address = model.Address,
                PreferredLearningStyle = model.PreferredLearningStyle,
                DevicePlatformUsed = model.DevicePlatformUsed,
                Gender = model.Gender,
                Profession = model.Profession,
                DateOfRegistration = DateTime.UtcNow
            };

            _logger.LogInformation("Saving demographics for user: " + user.Id);
            _context.UserDemographics.Add(demographics);
            await _context.SaveChangesAsync();

            var pin = new Random().Next(100000, 999999).ToString(); // Generates a 6-digit PIN
            await _emailService.SendSystemEmailAsync(user.Email, "Your 2FA PIN", $"Your 2FA PIN is: {pin}");

            // Store the PIN temporarily for later verification
            TempData["Email"] = user.Email;
            TempData["Pin"] = pin; // Store PIN temporarily to verify later
            return RedirectToAction("VerifyTwoFactor", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Json(new { exists = user != null });
        }
        public IActionResult VerifyTwoFactor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyTwoFactor(string pin)
        {
            var email = TempData["Email"]?.ToString();
            var storedPin = TempData["Pin"]?.ToString();  // Correctly retrieve the PIN

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(storedPin))
            {
                ModelState.AddModelError("", "Invalid token or session expired.");
                return View();
            }

            // Track the failed attempts using session
            int failedAttempts = HttpContext.Session.GetInt32(FailedAttemptsSessionKey) ?? 0;

            // If PIN is correct, proceed with registration
            if (pin == storedPin)
            {
                // Retrieve the user
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    // Confirm the email and enable 2FA for registration
                    user.EmailConfirmed = true;
                    user.TwoFactorEnabled = true;

                    // Save changes to user
                    await _userManager.UpdateAsync(user);

                    // Sign the user in (this will trigger 2FA)
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Disable 2FA for subsequent logins
                    user.TwoFactorEnabled = false;
                    await _userManager.UpdateAsync(user); // Disable 2FA after successful registration

                    TempData["SuccessMessage"] = "Registration successful!";
                    HttpContext.Session.SetInt32(FailedAttemptsSessionKey, 0);  // Reset failed attempts on success
                    return RedirectToAction("MyLearning", "Trainee");
                }
            }

            // If token is incorrect, increase the failed attempts counter
            failedAttempts++;
            HttpContext.Session.SetInt32(FailedAttemptsSessionKey, failedAttempts);

            // If the user has failed 3 times, delete the account
            if (failedAttempts >= MaxFailedAttempts)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    // Delete the user account
                    await _userManager.DeleteAsync(user);
                    TempData["ErrorMessage"] = "Account deleted due to multiple failed attempts. Please register again.";
                }

                // Reset failed attempts
                HttpContext.Session.SetInt32(FailedAttemptsSessionKey, 0);

                return RedirectToAction("Register");
            }

            ModelState.AddModelError("", "Invalid 2FA token.");
            return View();
        }


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
                        var userId = _userManager.GetUserId(User);
                        await _logUsageService.LogSystemUsageAsync(userId, "Login");

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
                            return RedirectToAction("CoordAnalyticsDashboard", "Coordinator");
                        }
                        else if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("AnalyticsDashboard", "Admin");
                        }
                        // Default redirect if no specific role
                        return RedirectToAction("Home", "Home");
                    }
                }
                else
                {
                    ViewBag.LoginFailed = true;
                }
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}