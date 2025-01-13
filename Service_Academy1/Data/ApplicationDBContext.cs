using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System;
using System.Reflection.Emit;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
    options)
    : base(options)
    {
    }
    public DbSet<DepartmentsModel> Departments { get; set; }
    public DbSet<ProgramsModel> Programs { get; set; }
    public DbSet<ProgramManagementModel> ProgramManagement { get; set; }
    public DbSet<EnrollmentModel> Enrollment { get; set; }
    public DbSet<ModuleModel> Modules { get; set; }
    public DbSet<QuizModel> Quizzes { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }
    public DbSet<AnswerModel> Answers { get; set; }
    public DbSet<ActivitiesModel> Activities { get; set; }
    public DbSet<TraineeActivitiesModel> TraineeActivities { get; set; }
    public DbSet<TraineeAnswerModel> TraineeAnswers { get; set; }
    public DbSet<TraineeQuizResultModel> TraineeQuizResults { get; set; }
    public DbSet<TraineeModuleResult> TraineeModuleResults { get; set; }
    public DbSet<AnnouncementModel> Announcements { get; set; }
    public DbSet<EvaluationCriteria> EvaluationCriteria { get; set; }
    public DbSet<EvaluationQuestionModel> EvaluationQuestions { get; set; }
    public DbSet<EvaluationResponseModel> EvaluationResponses { get; set; }
    public DbSet<SystemUsageLogModel> SystemUsageLogs { get; set; }
    public DbSet<UserDemographicsModel> UserDemographics { get; set; }
    public DbSet<CertificateModel> Certificates { get; set; }
    public DbSet<PostModel> Posts { get; set; }
    public DbSet<CommentModel> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Remove any HasData or other seed data logic from here!
        // Define your model structure, relationships, indexes, etc.
        builder.Entity<UserDemographicsModel>()
       .HasOne(d => d.ApplicationUser)
       .WithOne(u => u.UserDemographics)
       .HasForeignKey<UserDemographicsModel>(d => d.ApplicationUserId)
       .IsRequired();
    }
}
