
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class EvaluationQuestion
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public ProgramsModel? ProgramsModel { get; set; }
    }
}