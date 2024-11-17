using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class TraineeModuleResult
    {
        [Key]
        public int TraineeModuleResultId { get; set; }

        [ForeignKey("ModuleModel")]
        public int ModuleId { get; set; }

        [ForeignKey("EnrollmentModel")]
        public int EnrollmentId { get; set; } // Foreign key to reference the specific student enrollment
        public bool IsCompleted { get; set; } = false;
        public virtual ModuleModel? Modules { get; set; }
        public virtual EnrollmentModel? Enrollment { get; set; } // Navigation property
    }
}
