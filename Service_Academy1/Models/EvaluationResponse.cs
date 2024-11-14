using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class EvaluationResponse
    {
        [Key]
        public int ResponseId { get; set; }
        public int QuestionId { get; set; }
        public virtual EvaluationQuestion? EvaluationQuestion { get; set; }
        public int ProgramId { get; set; }
        public string LearnerId { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}