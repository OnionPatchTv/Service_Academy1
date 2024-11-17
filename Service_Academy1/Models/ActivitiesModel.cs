using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class ActivitiesModel
    {
        [Key]
        public int ActivitiesId { get; set; }
        [Required]
        public string ActivitiesTitle { get; set; } = string.Empty;
        [Required]
        public string ActivityDirection { get; set; } = string.Empty;
        [Required]
        public int TotalScore { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }

        public virtual ProgramsModel? ProgramsModel { get; set; } // Navigation property to program
        public virtual ICollection<TraineeActivitiesModel> TraineeActivities { get; set; } = [];
    }
}
