using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class EnrollmentModel
    {
        [Key]
        public int EnrollmentId { get; set; }

        [ForeignKey("CurrentTrainee")]
        public string? TraineeId { get; set; } // Foreign key property for current user

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; } // Foreign key for ProgramsModel or the current program
        public string EnrollmentStatus { get; set; } = string.Empty; //status if approved, pending, or denied
        public DateTime? EnrollmentDate { get; set; } //enrollment date
        public string ProgramStatus { get; set; } = string.Empty; //completion of the program default is incomplete

        public DateTime? StatusDate { get; set; } = DateTime.MinValue; // completiion date

        public string? ReasonForDenial { get; set; }
        // Navigation property to the instructor
        public virtual ApplicationUser? CurrentTrainee { get; set; }
        public virtual ProgramsModel? ProgramsModel { get; set; } // Navigation property
        public virtual ICollection<TraineeActivitiesModel> TraineeActivities { get; set; } = [];
        public virtual ICollection<TraineeModuleResult> TraineeModuleResults { get; set; } = [];
        public virtual ICollection<TraineeQuizResultModel> TraineeQuizResults { get; set; } = [];
        public virtual ICollection<EvaluationResponseModel> EvaluationResponses { get; set; } = [];

    }
}
