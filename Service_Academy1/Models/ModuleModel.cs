using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class ModuleModel
    {
        [Key]
        public int ModuleId { get; set; }

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }

        [Required]
        public string Title { get; set; }

        public string FilePath { get; set; }  // Stores the file path for the PDF/PPT file

        public virtual ProgramsModel? ProgramsModel { get; set; } // Navigation property
    }
}
