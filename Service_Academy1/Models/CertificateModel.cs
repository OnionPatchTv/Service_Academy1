using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class CertificateModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertificateId { get; set; }

        [Required]
        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; }
        public EnrollmentModel Enrollment { get; set; } // Navigation property

        [Required]
        public string CertificatePath { get; set; } //Make it non-nullable

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow; // Automatically sets the generation date
    }
}