using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Service_Academy1.Models
{
    public class EnrolleeViewModel
    {
        public int EnrollmentId { get; set; }
        public int ProgramId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string ProgramStatus { get; set; } = string.Empty;
        public string? ProfilePath { get; set; }
    }

}
