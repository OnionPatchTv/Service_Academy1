using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;

public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(ILogger<ProfileController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
    {
        (_logger, _context, _userManager, _signInManager) = (logger, context, userManager, signInManager);
    }

    public async Task<IActionResult> ProfilePage()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("No user is logged in.");
                return RedirectToAction("Login", "Account");
            }

            var userDemographics = await _context.UserDemographics
                .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUser.Id);

            var viewModel = new ProfileViewModel
            {
                UserDemographicsId = userDemographics?.Id ?? 0,
                FullName = currentUser.FullName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Address = userDemographics?.Address ?? string.Empty,
                DateOfBirth = userDemographics?.DateOfBirth,
                ProfilePath = userDemographics?.ProfilePath ?? "/images/default-profile.png",
                About = userDemographics?.About ?? "No Description"
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile page.");
            TempData["ProfileUpdateErrorMessage"] = "An error occurred while loading the profile page.";
            return RedirectToAction("Home", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model, IFormFile? profilePicInput)
    {
        if (!ModelState.IsValid)
        {
            TempData["ProfileUpdateErrorMessage"] = "Invalid input data.";
            return RedirectToAction("ProfilePage");
        }

        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ProfileUpdateErrorMessage"] = "No user is logged in.";
                return RedirectToAction("Login", "Account");
            }
            // Update Password Logic (if applicable)
            if (!string.IsNullOrWhiteSpace(model.CurrentPassword) &&
                !string.IsNullOrWhiteSpace(model.NewPassword) &&
                !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["ProfileUpdateErrorMessage"] = "New password and confirmation password do not match.";
                    return RedirectToAction("ProfilePage");
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(
                    currentUser,
                    model.CurrentPassword,
                    model.NewPassword
                );

                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        _logger.LogError("Error updating password: {Code} - {Description}", error.Code, error.Description);
                    }
                    TempData["ProfileUpdateErrorMessage"] = "Invalid current password or new password requirements not met.";
                    return RedirectToAction("ProfilePage");
                }
                TempData["ProfileUpdateSuccessMessage"] = "Password updated successfully.";
            }

            // Update user details
            if (!string.IsNullOrEmpty(model.FullName))
                currentUser.FullName = model.FullName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                currentUser.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.Email) && currentUser.Email != model.Email)
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == model.Email))
                {
                    TempData["ProfileUpdateErrorMessage"] = "Email is already in use.";
                    return RedirectToAction("ProfilePage");
                }

                currentUser.Email = model.Email;
                currentUser.UserName = model.Email;
                await _userManager.UpdateSecurityStampAsync(currentUser);
            }

            var updateResult = await _userManager.UpdateAsync(currentUser);
            if (!updateResult.Succeeded)
            {
                TempData["ProfileUpdateErrorMessage"] = "Error updating user details.";
                return RedirectToAction("ProfilePage");
            }

            // Update demographics
            var userDemographics = await _context.UserDemographics
                .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUser.Id);

            if (userDemographics == null)
            {
                userDemographics = new UserDemographicsModel
                {
                    ApplicationUserId = currentUser.Id,
                    Address = model.Address ?? string.Empty,
                    About = model.About ?? "No Description",
                    DateOfBirth = model.DateOfBirth?.ToUniversalTime() ?? DateTime.UtcNow
                };
                await _context.UserDemographics.AddAsync(userDemographics);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Address))
                    userDemographics.Address = model.Address;

                if (!string.IsNullOrEmpty(model.About))
                    userDemographics.About = model.About;

                if (model.DateOfBirth.HasValue)
                    userDemographics.DateOfBirth = model.DateOfBirth?.ToUniversalTime() ?? DateTime.UtcNow;
            }

            if (profilePicInput != null && profilePicInput.Length > 0)
            {
                var profileImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfileImage");
                Directory.CreateDirectory(profileImagePath);

                var fileName = Guid.NewGuid() + Path.GetExtension(profilePicInput.FileName);
                var filePath = Path.Combine(profileImagePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicInput.CopyToAsync(stream);
                }
                userDemographics.ProfilePath = "/ProfileImage/" + fileName;
            }

            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(currentUser);

            TempData["ProfileUpdateSuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("ProfilePage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during profile update.");
            TempData["ProfileUpdateErrorMessage"] = "An unexpected error occurred.";
            return RedirectToAction("ProfilePage");
        }
    }

}
