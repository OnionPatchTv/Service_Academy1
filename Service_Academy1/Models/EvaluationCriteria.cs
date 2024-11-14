using System.ComponentModel.DataAnnotations;

namespace Service_Academy1.Models
{
    public class EvaluationCriteria
    {
        [Key]
        public int CriteriaId { get; set; }

        [Required]
        public string CriteriaName { get; set; } = string.Empty;
    }
}