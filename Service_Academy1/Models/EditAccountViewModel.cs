using Microsoft.AspNetCore.Identity;
using Service_Academy1.Models;
using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class EditAccountViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}