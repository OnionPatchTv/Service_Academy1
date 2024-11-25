﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class ProgramsModel
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // This will hold the instructor's name for display purposes
        public string ProjectLeader { get; set; } = string.Empty;

        public string Agenda { get; set; } = string.Empty;
        public string SDG {  get; set; } = string.Empty;
        public string PhotoPath { get; set; } = string.Empty;

        // Foreign key property for the instructor's Id
        [ForeignKey("CurrentProjectLeader")]
        public string? ProjectLeaderId { get; set; } // Foreign key property

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        // Navigation property to the instructor
        public virtual ApplicationUser? CurrentProjectLeader { get; set; }
        public virtual DepartmentsModel? Department { get; set; }
        public virtual ICollection<ProgramManagementModel> ProgramManagement { get; set; } = [];
        public virtual ICollection<EnrollmentModel> Enrollments { get; set; } = [];
        public virtual ICollection<ModuleModel> Modules { get; set; } = [];
        public virtual ICollection<QuizModel> Quizzes { get; set; } = [];
        public virtual ICollection<ActivitiesModel> Activities { get; set; } = [];
        public virtual ICollection<EvaluationQuestionModel> EvaluationQuestions { get; set; } = [];
    }

}
