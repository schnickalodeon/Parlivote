using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class AddsApplicantIdToMotionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantId",
                table: "Motions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motions_ApplicantId",
                table: "Motions",
                column: "ApplicantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Motions_AspNetUsers_ApplicantId",
                table: "Motions",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motions_AspNetUsers_ApplicantId",
                table: "Motions");

            migrationBuilder.DropIndex(
                name: "IX_Motions_ApplicantId",
                table: "Motions");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "Motions");
        }
    }
}
