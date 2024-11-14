using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class ResultViewModel
    {
        public string? Category { get; set; }
        public double AverageRating { get; set; }
        public string? QuestionText { get; set; }
        public EvaluationQuestion? EvaluationQuestion { get; set; }
        public ProgramsModel? ProgramsModel { get; set; }
    }
}
