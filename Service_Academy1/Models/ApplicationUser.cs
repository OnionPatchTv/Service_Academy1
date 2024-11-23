using Microsoft.AspNetCore.Identity;
using Service_Academy1.Models;

public class ApplicationUser : IdentityUser
{
    // Nullable to handle cases where FullName or Role might not be set at object creation
    public string? FullName { get; set; }
    public int? DepartmentId { get; set; }
    public virtual ICollection<SystemUsageLogModel> SystemUsageLogs { get; set; } = new List<SystemUsageLogModel>();
    public virtual UserDemographicsModel UserDemographics { get; set; }
}
