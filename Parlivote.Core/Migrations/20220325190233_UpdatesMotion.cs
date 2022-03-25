using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class UpdatesMotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgendaItem",
                table: "Motions");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Motions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Motions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Motions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Motions");

            migrationBuilder.AddColumn<string>(
                name: "AgendaItem",
                table: "Motions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
