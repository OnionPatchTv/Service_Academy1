using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class EvaluationFormViewModel
    {
        public int ProgramId { get; set; }
        public List<EvaluationQuestionModel> Questions { get; set; } = [];
        public List<ResponseViewModel> Responses { get; set; } = [];
    }

    public class ResponseViewModel
    {
        public int EnrollmentId { get; set; }
        public int QuestionId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
    }

}
