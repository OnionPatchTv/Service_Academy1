
namespace Service_Academy1.Models
{
    public class EvaluationResultsViewModel
    {
        public string ProgramTitle { get; set; }
        public int ProgramId { get; set; }

        public List<ResultViewModel> AverageRatings { get; set; }
        public int EvaluatedCount { get; set; }
        public int TotalTrainees { get; set; }
        public int UnevaluatedCount { get; set; }

        public List<EvaluationQuestion> Questions { get; set; }
    }
}