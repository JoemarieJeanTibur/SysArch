using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tibur_LabAct1.Migrations
{
    /// <inheritdoc />
    public partial class AddApproveReject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdNumber",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdNumber",
                table: "Reservations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
