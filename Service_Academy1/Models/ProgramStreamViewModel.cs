using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Service_Academy1.Models
{
    public class ProgramStreamViewModel
    {
        public string Title { get; set; } = string.Empty;
        public int ProgramId { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string LinkPath { get; set; } = string.Empty;
        public bool IsArchived { get; set; } = false;
        public virtual ICollection<ModuleModel> Modules { get; set; } = [];
        public List<QuizModel> Quizzes { get; set; } = [];
        public List<EnrollmentModel> Enrollment { get; set; } = [];
        public List<ActivitiesModel> Activities { get; set; } = [];
        public AnnouncementModel? Announcement { get; set; }
    }
}
