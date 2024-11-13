using System;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty; // Initialized with default value

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty; // Initialized with default value

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                       ErrorMessage = "Passwords must have at least one lowercase 'a'-'z', one uppercase 'A'-'Z', one digit '0'-'9', one non-alphanumeric character, and at least 8 characters long.")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; } = "Trainee";

    // New fields for analytics

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty; // Initialized with default value

    [Required]
    [Display(Name = "Preferred Learning Style")]
    public string PreferredLearningStyle { get; set; } = string.Empty; // Initialized with default value

    [Required]
    [Display(Name = "Device/Platform Used")]
    public string DevicePlatformUsed { get; set; } = string.Empty; // Initialized with default value

    [Display(Name = "Date of Registration")]
    public DateTime DateOfRegistration { get; set; } = DateTime.Now; // Initialized with current date

    [Required]
    [Display(Name = "Gender")]
    public string Gender { get; set; } = string.Empty; // Initialized with default value

    [Required]
    [Display(Name = "Profession")]
    public string Profession { get; set; } = string.Empty; // Initialized with default value

}
