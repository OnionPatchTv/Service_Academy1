using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be at least {2} characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Passwords must have at least one lowercase, one uppercase, one digit, and one non-alphanumeric character.")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Trainee";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Preferred Learning Style")]
        public string PreferredLearningStyle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Device/Platform Used")]
        public string DevicePlatformUsed { get; set; } = string.Empty;

        [Display(Name = "Date of Registration")]
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Profession")]
        public string Profession { get; set; } = string.Empty;
    }
}