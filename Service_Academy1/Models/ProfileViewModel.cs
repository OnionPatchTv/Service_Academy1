namespace Service_Academy1.Models
{
    public class ProfileViewModel
    {
        public int UserDemographicsId { get; set; }
        public int UserId {  get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string?PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfilePath { get; set; } // This will store the image path
        public string? About { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

}
