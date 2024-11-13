using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
#nullable disable
namespace Service_Academy1.Migrations
{
    public partial class SeedRolesAndUsers : Migration
    {
        // Declare role and user IDs as fields (only once)
        private readonly string adminRoleId = Guid.NewGuid().ToString();
        private readonly string instructorRoleId =
        Guid.NewGuid().ToString();
        private readonly string studentRoleId = Guid.NewGuid().ToString();
        private readonly string adminUserId = Guid.NewGuid().ToString();
        private readonly string instructorUserId1 =
        Guid.NewGuid().ToString();
        private readonly string instructorUserId2 =
        Guid.NewGuid().ToString();
        private readonly string instructorUserId3 =
        Guid.NewGuid().ToString();
        private readonly string studentUserId1 =
        Guid.NewGuid().ToString();
        private readonly string studentUserId2 =
        Guid.NewGuid().ToString();
        private readonly string studentUserId3 =
        Guid.NewGuid().ToString();
        private readonly string studentUserId4 =
        Guid.NewGuid().ToString();
        private readonly string studentUserId5 =
        Guid.NewGuid().ToString();
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add roles (your existing role seeding code is correct)
            migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "Name", "NormalizedName",
"ConcurrencyStamp" },
            values: new object[,]
            {
{ adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString() },
{ instructorRoleId, "Instructor", "INSTRUCTOR",
Guid.NewGuid().ToString() },
{ studentRoleId, "Student", "STUDENT",
Guid.NewGuid().ToString() }
            }
            );
            // Create a password hasher instance to hash passwords
            var hasher = new PasswordHasher<ApplicationUser>();
            // Insert the users into the AspNetUsers table (include PhoneNumberConfirmed)
            migrationBuilder.InsertData(
            table: "AspNetUsers",
            columns: new[] {
"Id",
"UserName",
"NormalizedUserName",
"Email",
"NormalizedEmail",
"EmailConfirmed",
"PasswordHash",
"SecurityStamp",
"ConcurrencyStamp",
"PhoneNumber",
"PhoneNumberConfirmed",
"TwoFactorEnabled",
"LockoutEnd",
"LockoutEnabled",
"AccessFailedCount",
"FullName"
            },
            values: new object[,]
            {
{ adminUserId, "admin@academy.com", "ADMIN@ACADEMY.COM",
"admin@academy.com", "ADMIN@ACADEMY.COM", true, hasher.HashPassword(null,
"Admin123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null,
false, false, null, false, 0, "Admin User" },
{ instructorUserId1, "instructor1@academy.com",
"INSTRUCTOR1@ACADEMY.COM", "instructor1@academy.com",
"INSTRUCTOR1@ACADEMY.COM", true, hasher.HashPassword(null,
"Instructor123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
null, false, false, null, false, 0, "Instructor User 1" },
{ instructorUserId2, "instructor2@academy.com",
"INSTRUCTOR2@ACADEMY.COM", "instructor2@academy.com",
"INSTRUCTOR2@ACADEMY.COM", true, hasher.HashPassword(null,
"Instructor123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
null, false, false, null, false, 0, "Instructor User 2" },
{ instructorUserId3, "instructor3@academy.com",
"INSTRUCTOR3@ACADEMY.COM", "instructor3@academy.com",
"INSTRUCTOR3@ACADEMY.COM", true, hasher.HashPassword(null,
"Instructor123!"), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
null, false, false, null, false, 0, "Instructor User 3" },
{ studentUserId1, "student1@academy.com",
"STUDENT1@ACADEMY.COM", "student1@academy.com", "STUDENT1@ACADEMY.COM",
true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(),
Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 1" },
{ studentUserId2, "student2@academy.com",
"STUDENT2@ACADEMY.COM", "student2@academy.com", "STUDENT2@ACADEMY.COM",
true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(),
Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 2" },
{ studentUserId3, "student3@academy.com",
"STUDENT3@ACADEMY.COM", "student3@academy.com", "STUDENT3@ACADEMY.COM",
true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(),
Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 3" },
{ studentUserId4, "student4@academy.com",
"STUDENT4@ACADEMY.COM", "student4@academy.com", "STUDENT4@ACADEMY.COM",
true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(),
Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 4" },
{ studentUserId5, "student5@academy.com",
"STUDENT5@ACADEMY.COM", "student5@academy.com", "STUDENT5@ACADEMY.COM",
true, hasher.HashPassword(null, "Student123!"), Guid.NewGuid().ToString(),
Guid.NewGuid().ToString(), null, false, false, null, false, 0, "Student User 5" }
            }
            );
            // Add user-role assignments (your role assignment code is correct)
            migrationBuilder.InsertData(
            table: "AspNetUserRoles",
            columns: new[] { "UserId", "RoleId" },
            values: new object[,]
            {
{ adminUserId, adminRoleId },
{ instructorUserId1, instructorRoleId },
{ instructorUserId2, instructorRoleId },
{ instructorUserId3, instructorRoleId },
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
            // Reverse Role and User Seeding by deleting the inserted records
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { adminUserId, adminRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { instructorUserId1,
instructorRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { instructorUserId2,
instructorRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { instructorUserId3,
instructorRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { studentUserId1, studentRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { studentUserId2, studentRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { studentUserId3, studentRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { studentUserId4, studentRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUserRoles",
            keyColumns: new[] { "UserId", "RoleId" },
            keyValues: new object[] { studentUserId5, studentRoleId }
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: adminUserId
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: instructorUserId1
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: instructorUserId2
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: instructorUserId3
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: studentUserId1
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: studentUserId2
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: studentUserId3
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: studentUserId4
            );
            migrationBuilder.DeleteData(
            table: "AspNetUsers",
            keyColumn: "Id",
            keyValue: studentUserId5
            );
            migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: adminRoleId
            );
            migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: instructorRoleId
            );
            migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: studentRoleId
            );
        }
    }
}
