using Service_Academy1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class EvaluationQuestionModel
    {
        [Key]
        public int EvaluationQuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }
        public ProgramsModel? ProgramsModel { get; set; }

        public virtual ICollection<EvaluationResponseModel> Responses { get; set; } = new List<EvaluationResponseModel>();
    }
    public class EvaluationResponseModel
    {
        [Key]
        public int ResponseId { get; set; }

        [ForeignKey("EvaluationQuestions")]
        public int EvaluationQuestionId { get; set; }
        public virtual EvaluationQuestionModel? EvaluationQuestions { get; set; }

        [ForeignKey("EnrollmentModel")]
        public int EnrollmentId { get; set; } // Foreign key to reference the specific student enrollment
        public virtual EnrollmentModel Enrollment { get; set; } // Navigation property
        [Range(1, 5)]
        public int Rating { get; set; }
    }
    public class EvaluationCriteria
    {
        [Key]
        public int CriteriaId { get; set; }

        [Required]
        public string CriteriaName { get; set; } = string.Empty;
    }
}
