using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class SystemUsageLogModel
    {
        [Key]
        public int LogId { get; set; } // Primary key for the log

        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } // Foreign key to ApplicationUser

        public virtual ApplicationUser? User { get; set; } // Navigation property to ApplicationUser

        [Required]
        public string ActionType { get; set; } = string.Empty; // Type of action (e.g., "Login")

        public DateTime Timestamp { get; set; } = DateTime.Now; // When the action happened

        public int? TargetId { get; set; } // Optional: ID of the associated resource (e.g., ProgramId)
    }
}

