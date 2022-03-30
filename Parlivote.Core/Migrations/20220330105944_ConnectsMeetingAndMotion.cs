using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class ConnectsMeetingAndMotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MeetingId",
                table: "Motions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motions_MeetingId",
                table: "Motions",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions");

            migrationBuilder.DropIndex(
                name: "IX_Motions_MeetingId",
                table: "Motions");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "Motions");
        }
    }
}
