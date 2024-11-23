using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class AnnouncementModel
    {
        [Key]
        public int AnnouncementId { get; set; }

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }

        public string AnnouncementTitle { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public virtual ProgramsModel? ProgramsModel { get; set; } // Navigation property to program
    }
}