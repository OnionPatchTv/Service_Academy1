using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
