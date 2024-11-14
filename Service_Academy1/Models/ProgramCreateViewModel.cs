using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class ProgramCreateViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Agenda { get; set; } = string.Empty;
        public string ProjectLeaderId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string ProjectLeader { get; set; } = string.Empty; // This will be set in the controller
        public string DepartmentName { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
    }
}
