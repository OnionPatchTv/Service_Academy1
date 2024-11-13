using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class ProgramsModel
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // This will hold the instructor's name for display purposes
        public string Instructor { get; set; } = string.Empty;

        public string Agenda { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;

        // Foreign key property for the instructor's Id
        [ForeignKey("currentInstructor")]
        public string? InstructorId { get; set; } // Foreign key property

        // Navigation property to the instructor
        public virtual ApplicationUser? currentInstructor { get; set; }
        public virtual ICollection<ProgramManagementModel> ProgramManagement { get; set; } = new List<ProgramManagementModel>();
        public virtual ICollection<EnrollmentModel> Enrollments { get; set; } = new List<EnrollmentModel>();
        public virtual ICollection<ModuleModel> Modules { get; set; } = new List<ModuleModel>();
        public virtual ICollection<QuizModel> Quizzes { get; set; } = new List<QuizModel>();
    }

}
