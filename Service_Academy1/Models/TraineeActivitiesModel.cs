using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class TraineeActivitiesModel
    {
        [Key]
        public int TraineeActivityId { get; set; }
        [ForeignKey("ActivitiesModel")]
        public int ActivitiesId { get; set; }
        [ForeignKey("EnrollmentModel")]
        public int EnrollmentId { get; set; }
        [Required]
        public string FilePath { get; set; } = string.Empty;
        public int RawScore {  get; set; }
        public DateTime SubmittedAt { get; set; }
        public int ComputedScore { get; set; }
        public bool IsCompleted { get; set; } = false;
       // Foreign key to reference the specific student enrollment
        public virtual EnrollmentModel Enrollment { get; set; } // Navigation pro
        public virtual ActivitiesModel Activities { get; set; }
    }
}
