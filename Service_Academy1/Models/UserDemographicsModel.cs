using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class UserDemographicsModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(ApplicationUser))]
    public string? ApplicationUserId { get; set; } // Ensure this matches the ApplicationUser primary key type

    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfRegistration { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PreferredLearningStyle { get; set; } = string.Empty;
    public string DevicePlatformUsed { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty;
    public string? ProfilePath { get; set; }
    public string? About {  get; set; } = "No Description";

    // Navigation property
    public virtual ApplicationUser? ApplicationUser { get; set; }
}
