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
   
        public string FilePath { get; set; } = "No Document";
        public string LinkPath { get; set; } = "No Link Pasted";
        public int RawScore {  get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.MinValue;
        public int ComputedScore { get; set; }
        public bool IsCompleted { get; set; } = false;
       // Foreign key to reference the specific student enrollment
        public virtual EnrollmentModel? Enrollment { get; set; } // Navigation pro
        public virtual ActivitiesModel? Activities { get; set; }
    }
}
