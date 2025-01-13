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
using Microsoft.AspNetCore.Identity.UI.Services;


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
            HttpContext.Session.SetString("Pin", pin);
            HttpContext.Session.SetString("Email", user.Email);
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
            var email = HttpContext.Session.GetString("Email");
            var storedPin = HttpContext.Session.GetString("Pin");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(storedPin))
            {
                ModelState.AddModelError("", "Invalid token or session expired.");
                return View();
            }

            // Verify the PIN entered by the user
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
                    return RedirectToAction("MyLearning", "Trainee");
                }
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
        // GET: Forgot Password
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "No account found with this email.";
                    return RedirectToAction("ForgotPassword");
                }

                // Generate password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Generate a password reset URL
                var resetLink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = token, email = user.Email },
                    protocol: HttpContext.Request.Scheme);

                // Send email (using your email sender service)
                await _emailService.SendSystemEmailAsync(user.Email, "Password Reset", $"Click <a href='{resetLink}'>here</a> to reset your password.");

                TempData["SuccessMessage"] = "Password reset link sent to your email.";
                return RedirectToAction("Login", "Account");
            }

            // In case of validation error
            return View(model);
        }
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        // POST: Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "No account found with this email.";
                    return RedirectToAction("ForgotPassword");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password reset successfully.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error resetting password.";
                    return RedirectToAction("ResetPassword", new { token = model.Token, email = model.Email });
                }
            }

            return View(model);
        }
    }
}