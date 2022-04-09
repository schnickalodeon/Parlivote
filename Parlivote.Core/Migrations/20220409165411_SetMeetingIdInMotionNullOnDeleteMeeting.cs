using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class SetMeetingIdInMotionNullOnDeleteMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions");

            migrationBuilder.AddForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions");

            migrationBuilder.AddForeignKey(
                name: "FK_Motions_Meetings_MeetingId",
                table: "Motions",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id");
        }
    }
}
