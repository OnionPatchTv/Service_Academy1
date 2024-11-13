using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
    options)
    : base(options)
    {
    }
    public DbSet<ProgramsModel> Programs { get; set; }
    public DbSet<ProgramManagementModel> ProgramManagement { get; set; }
    public DbSet<EnrollmentModel> Enrollment { get; set; }
    public DbSet<ModuleModel> Modules { get; set; }
    public DbSet<QuizModel> Quizzes { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }
    public DbSet<AnswerModel> Answers { get; set; }
    public DbSet<StudentAnswerModel> StudentAnswers { get; set; }
    public DbSet<StudentQuizResultModel> StudentQuizResults { get; set; }
    public DbSet<AnnouncementModel> Announcemnets { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Remove any HasData or other seed data logic from here!
        // Define your model structure, relationships, indexes, etc.
    }
}
