namespace Service_Academy1.Models
{
    public class EvaluationFormViewModel
    {
        public int ProgramId { get; set; }
        public List<EvaluationQuestion>? Questions { get; set; }
        public List<ResponseViewModel>? Responses { get; set; }
    }

    public class ResponseViewModel
    {
        public int QuestionId { get; set; }
        public int Rating { get; set; }
    }


    public class TraineeViewModel
    {

        public int ProgramId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EnrollmentModel>? Enrollments { get; set; }

        public List<ModuleModel>? Modules { get; set; }
        public List<QuizModel>? Quizzes { get; set; }
        public List<EvaluationResponse>? Evaluations { get; set; }
        public List<ProgramManagementModel>? Schedules { get; set; }


    }

}