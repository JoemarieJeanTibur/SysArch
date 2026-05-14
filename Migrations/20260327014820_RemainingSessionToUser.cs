using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tibur_LabAct1.Migrations
{
    /// <inheritdoc />
    public partial class RemainingSessionToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingSession",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingSession",
                table: "Students");
        }
    }
}
