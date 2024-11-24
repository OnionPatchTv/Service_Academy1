using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class ProfileImageActionFilter : IActionFilter
{
    private readonly ApplicationDbContext _context;

    // Constructor to inject the ApplicationDbContext
    public ProfileImageActionFilter(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        if (controller != null && controller.User.Identity.IsAuthenticated)
        {
            string profilePath = GetProfileImagePathForUser(controller.User.Identity.Name);
            controller.ViewData["ProfilePath"] = profilePath;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private string GetProfileImagePathForUser(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == username);
        if (user != null)
        {
            var userDemographics = _context.UserDemographics.FirstOrDefault(d => d.ApplicationUserId == user.Id);
            return userDemographics?.ProfilePath ?? ""; // Return the profile path or an empty string if not found
        }
        return ""; // Return an empty string if user is not found
    }
    private string GetUserProfileImage(string username)
    {
        // Simulate a call to a database or service to retrieve the profile image for a user
        // Example: return _dbContext.Users.Where(u => u.Username == username).Select(u => u.ProfileImage).FirstOrDefault();
        return null; // Return null if no profile image exists.
    }
}
