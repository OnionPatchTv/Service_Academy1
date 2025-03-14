using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class CreateAccountViewModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Display(Name = "FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department")]
        public string? Department { get; set; }
    }

}