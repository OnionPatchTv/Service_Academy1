﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Service_Academy1.Models
{
    public class QuizModel
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public string QuizTitle { get; set; } = string.Empty;

        [Required]
        public string QuizDirection { get; set; } = string.Empty;

        [ForeignKey("ProgramsModel")]
        public int ProgramId { get; set; }

        public virtual ProgramsModel? ProgramsModel { get; set; } // Navigation property to program

        public virtual ICollection<QuestionModel> Questions { get; set; } = []; // One-to-many relationship with QuestionModel
    }

    public class QuestionModel
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty ;

        [ForeignKey("QuizModel")]
        public int QuizId { get; set; }

        public virtual QuizModel? Quiz { get; set; } // Navigation property

        // Add the CorrectAnswer property to store the correct answer in the database
        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;// Correct answer for the question

        public virtual ICollection<AnswerModel> Answers { get; set; } = []; // One-to-many relationship with AnswerModel
    }

    public class AnswerModel
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public string Answer { get; set; } = string.Empty; // Option text

        [ForeignKey("QuestionModel")]
        public int QuestionId { get; set; }

        public virtual QuestionModel? Question { get; set; } // Navigation property
    }
}

