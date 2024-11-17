using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class TraineeQuizResultModel
    {
        [Key]
        public int TraineeQuizResultId { get; set; }

        [ForeignKey("QuizModel")]
        public int QuizId { get; set; }

        [ForeignKey("EnrollmentModel")]
        public int EnrollmentId { get; set; } // Foreign key to reference the specific student enrollment

        [Required]
        public int RawScore { get; set; } // Number of correct answers

        [Required]
        public int TotalScore { get; set; } // Total number of questions in the quiz

        [Required]
        public double ComputedScore { get; set; } // Score calculated as (RawScore / TotalScore * 63.5 + 37.5)

        [Required]
        public string Remarks { get; set; } = string.Empty;  // Pass or Fail based on score or other criteria

        public int Retries {  get; set; }
        public bool IsCompleted { get; set; } = false;
        public virtual QuizModel? Quiz { get; set; }
        public virtual EnrollmentModel? Enrollment { get; set; } // Navigation property
        public virtual ICollection<TraineeAnswerModel> TraineeAnswers { get; set; } = []; // Answers submitted by the student
    }
}
