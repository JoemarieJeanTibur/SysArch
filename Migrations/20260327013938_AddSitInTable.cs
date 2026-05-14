using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tibur_LabAct1.Migrations
{
    /// <inheritdoc />
    public partial class AddSitInTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SitIn",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lab = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemainingSession = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitIn", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SitIn");
        }
    }
}
