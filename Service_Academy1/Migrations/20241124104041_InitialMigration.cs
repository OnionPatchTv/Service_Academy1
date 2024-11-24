using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service_Academy1.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Department = table.Column<string>(type: "text", nullable: false),
                    DepartmentName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationCriteria",
                columns: table => new
                {
                    CriteriaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CriteriaName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCriteria", x => x.CriteriaId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsageLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ActionType = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TargetId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsageLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_SystemUsageLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDemographics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateOfRegistration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PreferredLearningStyle = table.Column<string>(type: "text", nullable: false),
                    DevicePlatformUsed = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Profession = table.Column<string>(type: "text", nullable: false),
                    ProfilePath = table.Column<string>(type: "text", nullable: true),
                    About = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDemographics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDemographics_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    ProgramId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ProjectLeader = table.Column<string>(type: "text", nullable: false),
                    Agenda = table.Column<string>(type: "text", nullable: false),
                    SDG = table.Column<string>(type: "text", nullable: false),
                    PhotoPath = table.Column<string>(type: "text", nullable: false),
                    ProjectLeaderId = table.Column<string>(type: "text", nullable: true),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.ProgramId);
                    table.ForeignKey(
                        name: "FK_Programs_AspNetUsers_ProjectLeaderId",
                        column: x => x.ProjectLeaderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Programs_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivitiesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivitiesTitle = table.Column<string>(type: "text", nullable: false),
                    ActivityDirection = table.Column<string>(type: "text", nullable: false),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProgramId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivitiesId);
                    table.ForeignKey(
                        name: "FK_Activities_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramId = table.Column<int>(type: "integer", nullable: false),
                    AnnouncementTitle = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.AnnouncementId);
                    table.ForeignKey(
                        name: "FK_Announcements_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TraineeId = table.Column<string>(type: "text", nullable: true),
                    ProgramId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentStatus = table.Column<string>(type: "text", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgramStatus = table.Column<string>(type: "text", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReasonForDenial = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollment_AspNetUsers_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Enrollment_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationQuestions",
                columns: table => new
                {
                    EvaluationQuestionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    ProgramId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationQuestions", x => x.EvaluationQuestionId);
                    table.ForeignKey(
                        name: "FK_EvaluationQuestions_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    LinkPath = table.Column<string>(type: "text", nullable: false),
                    ModuleDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                    table.ForeignKey(
                        name: "FK_Modules_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramManagement",
                columns: table => new
                {
                    ProgramManagementId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<string>(type: "text", nullable: false),
                    ReasonForDenial = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramManagement", x => x.ProgramManagementId);
                    table.ForeignKey(
                        name: "FK_ProgramManagement_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizTitle = table.Column<string>(type: "text", nullable: false),
                    QuizDirection = table.Column<string>(type: "text", nullable: false),
                    ProgramId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_Quizzes_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "ProgramId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    CertificatePath = table.Column<string>(type: "text", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeActivities",
                columns: table => new
                {
                    TraineeActivityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivitiesId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    LinkPath = table.Column<string>(type: "text", nullable: false),
                    RawScore = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ComputedScore = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeActivities", x => x.TraineeActivityId);
                    table.ForeignKey(
                        name: "FK_TraineeActivities_Activities_ActivitiesId",
                        column: x => x.ActivitiesId,
                        principalTable: "Activities",
                        principalColumn: "ActivitiesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeActivities_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationResponses",
                columns: table => new
                {
                    ResponseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvaluationQuestionId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationResponses", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_EvaluationResponses_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluationResponses_EvaluationQuestions_EvaluationQuestionId",
                        column: x => x.EvaluationQuestionId,
                        principalTable: "EvaluationQuestions",
                        principalColumn: "EvaluationQuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeModuleResults",
                columns: table => new
                {
                    TraineeModuleResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeModuleResults", x => x.TraineeModuleResultId);
                    table.ForeignKey(
                        name: "FK_TraineeModuleResults_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeModuleResults_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "text", nullable: false),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeQuizResults",
                columns: table => new
                {
                    TraineeQuizResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuizId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    RawScore = table.Column<int>(type: "integer", nullable: false),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    ComputedScore = table.Column<double>(type: "double precision", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    Retries = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeQuizResults", x => x.TraineeQuizResultId);
                    table.ForeignKey(
                        name: "FK_TraineeQuizResults_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeQuizResults_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeAnswers",
                columns: table => new
                {
                    TraineeAnswerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TraineeQuizResultId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeAnswers", x => x.TraineeAnswerId);
                    table.ForeignKey(
                        name: "FK_TraineeAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeAnswers_TraineeQuizResults_TraineeQuizResultId",
                        column: x => x.TraineeQuizResultId,
                        principalTable: "TraineeQuizResults",
                        principalColumn: "TraineeQuizResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProgramId",
                table: "Activities",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_ProgramId",
                table: "Announcements",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_EnrollmentId",
                table: "Certificates",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_ProgramId",
                table: "Enrollment",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_TraineeId",
                table: "Enrollment",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationQuestions_ProgramId",
                table: "EvaluationQuestions",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResponses_EnrollmentId",
                table: "EvaluationResponses",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationResponses_EvaluationQuestionId",
                table: "EvaluationResponses",
                column: "EvaluationQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_ProgramId",
                table: "Modules",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramManagement_ProgramId",
                table: "ProgramManagement",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_DepartmentId",
                table: "Programs",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ProjectLeaderId",
                table: "Programs",
                column: "ProjectLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_ProgramId",
                table: "Quizzes",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsageLogs_UserId",
                table: "SystemUsageLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeActivities_ActivitiesId",
                table: "TraineeActivities",
                column: "ActivitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeActivities_EnrollmentId",
                table: "TraineeActivities",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAnswers_QuestionId",
                table: "TraineeAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAnswers_TraineeQuizResultId",
                table: "TraineeAnswers",
                column: "TraineeQuizResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeModuleResults_EnrollmentId",
                table: "TraineeModuleResults",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeModuleResults_ModuleId",
                table: "TraineeModuleResults",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeQuizResults_EnrollmentId",
                table: "TraineeQuizResults",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeQuizResults_QuizId",
                table: "TraineeQuizResults",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDemographics_ApplicationUserId",
                table: "UserDemographics",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "EvaluationCriteria");

            migrationBuilder.DropTable(
                name: "EvaluationResponses");

            migrationBuilder.DropTable(
                name: "ProgramManagement");

            migrationBuilder.DropTable(
                name: "SystemUsageLogs");

            migrationBuilder.DropTable(
                name: "TraineeActivities");

            migrationBuilder.DropTable(
                name: "TraineeAnswers");

            migrationBuilder.DropTable(
                name: "TraineeModuleResults");

            migrationBuilder.DropTable(
                name: "UserDemographics");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "EvaluationQuestions");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "TraineeQuizResults");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
