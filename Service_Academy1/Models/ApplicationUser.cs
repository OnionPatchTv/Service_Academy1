using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Nullable to handle cases where FullName or Role might not be set at object creation
    public string? FullName { get; set; }
    public int? DepartmentId { get; set; }
}
