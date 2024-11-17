using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class TraineeAnswerModel
    {
        [Key]
        public int TraineeAnswerId { get; set; }

        [ForeignKey("TraineeQuizResultModel")]
        public int TraineeQuizResultId { get; set; } // Foreign key to the quiz result

        [ForeignKey("QuestionModel")]
        public int QuestionId { get; set; } // Foreign key to reference the question

        [Required]
        public string Answer { get; set; } = string.Empty; // Student’s answer to the question

        public bool IsCorrect { get; set; } // Whether the student's answer is correct
        public virtual TraineeQuizResultModel? TraineeQuizResult { get; set; } // Navigation property
        public virtual QuestionModel? Question { get; set; } // Navigation property
    }
}
