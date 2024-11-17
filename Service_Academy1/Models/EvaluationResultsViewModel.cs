using System.Collections.Generic;

namespace Service_Academy1.Models
{
    public class EvaluationResultsViewModel
    {
        public string ProgramTitle { get; set; } = string.Empty;
        public int TotalTrainees { get; set; }
        public int EvaluatedCount { get; set; }
        public int UnevaluatedCount { get; set; }
        public int ProgramId { get; set; }
        public List<AverageRatingViewModel> AverageRatings { get; set; } = [];
        public List<EvaluationResponseDetail> EvaluationDetails { get; set; } = [];
    }

    public class AverageRatingViewModel
    {
        public string Category { get; set; } = string.Empty;
        public double AverageRating { get; set; }
    }

    public class EvaluationResponseDetail
    {
        public string Category { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int Count { get; set; }
    }

}