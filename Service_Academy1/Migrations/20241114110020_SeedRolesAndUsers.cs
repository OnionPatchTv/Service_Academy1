using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Service_Academy1.Migrations
{
    public partial class SeedRolesAndUsers : Migration
    {
        // Role IDs
        private const string adminRoleId = "ROLE_ADMIN";
        private const string coordinatorRoleId = "ROLE_COORDINATOR";
        private const string projectLeaderRoleId = "ROLE_PROJECTLEADER"; // Renamed role
        private const string studentRoleId = "ROLE_STUDENT";

        // User IDs
        private const string adminUserId = "USER_ADMIN";
        private const string coordinatorUserId1 = "USER_COORDINATOR_1";
        private const string coordinatorUserId2 = "USER_COORDINATOR_2";
        private const string coordinatorUserId3 = "USER_COORDINATOR_3"; // Added for scalability
        private const string coordinatorUserId4 = "USER_COORDINATOR_4"; // Added for scalability
        private const string projectLeaderUserId1 = "USER_PROJECTLEADER_1";
        private const string projectLeaderUserId2 = "USER_PROJECTLEADER_2";
        private const string projectLeaderUserId3 = "USER_PROJECTLEADER_3"; // Added for scalability
        private const string projectLeaderUserId4 = "USER_PROJECTLEADER_4"; // Added for scalability
        private const string projectLeaderUserId5 = "USER_PROJECTLEADER_5"; // Added for scalability
        private const string projectLeaderUserId6 = "USER_PROJECTLEADER_6"; // Added for scalability
        private const string studentUserId1 = "USER_STUDENT_1";
        private const string studentUserId2 = "USER_STUDENT_2";
        private const string studentUserId3 = "USER_STUDENT_3";
        private const string studentUserId4 = "USER_STUDENT_4";
        private const string studentUserId5 = "USER_STUDENT_5";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Roles
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    { adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString() },
                    { coordinatorRoleId, "Coordinator", "COORDINATOR", Guid.NewGuid().ToString() },
                    { projectLeaderRoleId, "ProjectLeader", "PROJECTLEADER", Guid.NewGuid().ToString() }, // Renamed role
                    { studentRoleId, "Student", "STUDENT", Guid.NewGuid().ToString() }
                }
            );

            var hasher = new PasswordHasher<ApplicationUser>();

            // Seed Departments (Scalable approach)
            // Add more departments as needed
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "Department", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "CAFAD", "College of Architecture, Fine Arts and Design" },
                    { 2, "COE", "College of Engineering" },
                    { 3, "CICS", "College of Informatics and Computing Sciences" },
                    { 4, "CET", "College of Engineering Technology (CET)" }
                }
            );

            // Seed Coordinators with Department (Scalable approach)
            // This seeds one Coordinator for each department
            var departmentIds = new[] { 1, 2, 3, 4 }; // Array of department IDs
            for (int i = 0; i < departmentIds.Length; i++)
            {
                int departmentId = departmentIds[i];
                string coordinatorUserId = $"USER_COORDINATOR_{i + 1}";

                migrationBuilder.InsertData(
                    table: "AspNetUsers",
                    columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName", "DepartmentId" },
                    values: new object[]
                    {
                        coordinatorUserId,
                        $"coordinator{i + 1}@academy.com",
                        $"COORDINATOR{i + 1}@ACADEMY.COM",
                        $"coordinator{i + 1}@academy.com",
                        $"COORDINATOR{i + 1}@ACADEMY.COM",
                        true,
                        hasher.HashPassword(null, "Coordinator123!"),
                        Guid.NewGuid().ToString(),
                        Guid.NewGuid().ToString(),
                        null,
                        false,
                        false,
                        null,
                        false,
                        0,
                        $"Coordinator User {i + 1}",
                        departmentId
                    }
                );
            }

            // Seed Project Leaders with Department (Scalable approach)
            // This seeds two ProjectLeaders for each department
            for (int i = 0; i < departmentIds.Length; i++)
            {
                int departmentId = departmentIds[i];
                for (int j = 0; j < 2; j++)
                {
                    string projectLeaderUserId = $"USER_PROJECTLEADER_{i * 2 + j + 1}";

                    migrationBuilder.InsertData(
                        table: "AspNetUsers",
                        columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName", "DepartmentId" },
                        values: new object[]
                        {
                            projectLeaderUserId,
                            $"projectleader{i * 2 + j + 1}@academy.com",
                            $"PROJECTLEADER{i * 2 + j + 1}@ACADEMY.COM",
                            $"projectleader{i * 2 + j + 1}@academy.com",
                            $"PROJECTLEADER{i * 2 + j + 1}@ACADEMY.COM",
                            true,
                            hasher.HashPassword(null, "ProjectLeader123!"),
                            Guid.NewGuid().ToString(),
                            Guid.NewGuid().ToString(),
                            null,
                            false,
                            false,
                            null,
                            false,
                            0,
                            $"ProjectLeader User {i * 2 + j + 1}",
                            departmentId
                        }
                    );
                }
            }

            // Seed Admins (without Department)
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName" },
                values: new object[]
                {
                    adminUserId,
                    "admin@academy.com",
                    "ADMIN@ACADEMY.COM",
                    "admin@academy.com",
                    "ADMIN@ACADEMY.COM",
                    true,
                    hasher.HashPassword(null, "Admin123!"),
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    null,
                    false,
                    false,
                    null,
                    false,
                    0,
                    "Admin User"
                }
            );

            // Seed Students (without Department)
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount", "FullName" },
                values: new object[,]
                {
                    { studentUserId1, "student1@academy.com", "STUDENT1@ACADEMY.COM", "student1@academy.com", "STUDENT1@ACADEMY.COM", true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 1" },
                    { studentUserId2, "student2@academy.com", "STUDENT2@ACADEMY.COM", "student2@academy.com", "STUDENT2@ACADEMY.COM", true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 2" },
                    { studentUserId3, "student3@academy.com", "STUDENT3@ACADEMY.COM", "student3@academy.com", "STUDENT3@ACADEMY.COM", true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 3" },
                    { studentUserId4, "student4@academy.com", "STUDENT4@ACADEMY.COM", "student4@academy.com", "STUDENT4@ACADEMY.COM", true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 4" },
                    { studentUserId5, "student5@academy.com", "STUDENT5@ACADEMY.COM", "student5@academy.com", "STUDENT5@ACADEMY.COM", true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 5" }
                }
            );

            // Seed User Roles
            // Assign roles to each user based on their role
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { adminUserId, adminRoleId },
                    { coordinatorUserId1, coordinatorRoleId },
                    { coordinatorUserId2, coordinatorRoleId },
                    { coordinatorUserId3, coordinatorRoleId }, // Added for scalability
                    { coordinatorUserId4, coordinatorRoleId }, // Added for scalability
                    { projectLeaderUserId1, projectLeaderRoleId }, // Use renamed role ID
                    { projectLeaderUserId2, projectLeaderRoleId }, // Use renamed role ID
                    { projectLeaderUserId3, projectLeaderRoleId }, // Added for scalability
                    { projectLeaderUserId4, projectLeaderRoleId }, // Added for scalability
                    { projectLeaderUserId5, projectLeaderRoleId }, // Added for scalability
                    { projectLeaderUserId6, projectLeaderRoleId }, // Added for scalability
                    { studentUserId1, studentRoleId },
                    { studentUserId2, studentRoleId },
                    { studentUserId3, studentRoleId },
                    { studentUserId4, studentRoleId },
                    { studentUserId5, studentRoleId }
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the seeding process
            // Delete data in reverse order of seeding

            // Delete User Roles
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { adminUserId, adminRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { coordinatorUserId1, coordinatorRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { coordinatorUserId2, coordinatorRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { coordinatorUserId3, coordinatorRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { coordinatorUserId4, coordinatorRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId1, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId2, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId3, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId4, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId5, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { projectLeaderUserId6, projectLeaderRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { studentUserId1, studentRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { studentUserId2, studentRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { studentUserId3, studentRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { studentUserId4, studentRoleId });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { studentUserId5, studentRoleId });

            // Delete Users
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: adminUserId);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: coordinatorUserId1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: coordinatorUserId2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: coordinatorUserId3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: coordinatorUserId4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId5);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: projectLeaderUserId6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: studentUserId1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: studentUserId2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: studentUserId3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: studentUserId4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: studentUserId5);

            // Delete Roles
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: adminRoleId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: coordinatorRoleId);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: projectLeaderRoleId); // Delete renamed role

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: studentRoleId);

            // Delete Departments
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4);
        }
    }
}