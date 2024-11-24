using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;

public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(ILogger<ProfileController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        (_logger, _context, _userManager) = (logger, context, userManager);
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
        _logger.LogInformation("Starting profile update for user.");

        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error: {Key} - {Message}", state.Key, error.ErrorMessage);
                }
            }

            TempData["ProfileUpdateErrorMessage"] = "Invalid input data.";
            return RedirectToAction("ProfilePage");
        }

        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                _logger.LogWarning("No user is logged in.");
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

            // Update ApplicationUser details
            currentUser.FullName = model.FullName;
            currentUser.Email = model.Email;
            currentUser.PhoneNumber = model.PhoneNumber;
            var userUpdateResult = await _userManager.UpdateAsync(currentUser);

            if (!userUpdateResult.Succeeded)
            {
                foreach (var error in userUpdateResult.Errors)
                {
                    _logger.LogError("Error updating user: {Code} - {Description}", error.Code, error.Description);
                }
                TempData["ProfileUpdateErrorMessage"] = "Error updating user details.";
                return RedirectToAction("ProfilePage");
            }

            // Update or Create UserDemographics
            var userDemographics = await _context.UserDemographics
                .FirstOrDefaultAsync(d => d.Id == model.UserDemographicsId && d.ApplicationUserId == currentUser.Id);

            if (userDemographics == null)
            {
                _logger.LogInformation("No UserDemographics found. Creating a new one.");
                userDemographics = new UserDemographicsModel
                {
                    ApplicationUserId = currentUser.Id,
                    Address = model.Address,
                    About = model.About,
                    DateOfBirth = model.DateOfBirth.HasValue
                        ? DateTime.SpecifyKind(model.DateOfBirth.Value, DateTimeKind.Utc)
                        : DateTime.UtcNow,
                };
                _context.UserDemographics.Add(userDemographics);
            }
            else
            {
                _logger.LogInformation("Updating existing UserDemographics for user.");
                userDemographics.Address = model.Address;
                userDemographics.About = model.About;

                // Ensure DateOfBirth is converted to UTC
                userDemographics.DateOfBirth = model.DateOfBirth.HasValue
                    ? DateTime.SpecifyKind(model.DateOfBirth.Value, DateTimeKind.Utc)
                    : userDemographics.DateOfBirth;
            }

            // Handle Profile Picture Upload (Optional)
            if (profilePicInput != null && profilePicInput.Length > 0)
            {
                _logger.LogInformation("Processing uploaded profile picture.");
                var profileImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfileImage");
                if (!Directory.Exists(profileImagePath))
                {
                    Directory.CreateDirectory(profileImagePath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(profilePicInput.FileName);
                var filePath = Path.Combine(profileImagePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicInput.CopyToAsync(stream);
                }
                userDemographics.ProfilePath = "/ProfileImage/" + fileName;
            }
            else
            {
                _logger.LogInformation("No profile picture uploaded, keeping existing profile picture.");
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Profile updated successfully for user: {UserId}", currentUser.Id);

            TempData["ProfileUpdateSuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("ProfilePage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile.");
            TempData["ProfileUpdateErrorMessage"] = "An unexpected error occurred while updating your profile.";
            return RedirectToAction("ProfilePage");
        }
    }


}
