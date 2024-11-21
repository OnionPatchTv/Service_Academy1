using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service_Academy1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModuleDescription",
                table: "Modules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleDescription",
                table: "Modules");
        }
    }
}
