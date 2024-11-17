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
        public virtual TraineeQuizResultModel TraineeQuizResult { get; set; } // Navigation property

        [ForeignKey("QuestionModel")]
        public int QuestionId { get; set; } // Foreign key to reference the question
        public virtual QuestionModel Question { get; set; } // Navigation property

        [Required]
        public string Answer { get; set; } // Student’s answer to the question

        public bool IsCorrect { get; set; } // Whether the student's answer is correct
    }
}
