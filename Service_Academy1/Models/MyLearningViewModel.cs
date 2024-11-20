namespace Service_Academy1.Models
{
    public class MyLearningViewModel
    {
        public int ProgramId { get; set; }
        public string Title { get; set; }  = string.Empty;
        public string Agenda { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public string ProgramStatus { get; set; } = string.Empty;
        public string ReasonForDenial { get; set; } = string.Empty;
        public bool IsArchived { get; set; }
        public double ModulesProgress { get; set; } // Percentage
        public double ActivitiesProgress { get; set; } // Percentage
        public double QuizzesProgress { get; set; } // Percentage
        public double EvaluationProgress { get; set; } // Percentage
        public double TotalProgress => ModulesProgress + ActivitiesProgress + QuizzesProgress + EvaluationProgress;

    }
}
