using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class SetUserIdAndMotionIdToAlternateKeyInVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_MotionId",
                table: "Votes");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Votes_MotionId_UserId",
                table: "Votes",
                columns: new[] { "MotionId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Votes_MotionId_UserId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_MotionId",
                table: "Votes",
                column: "MotionId");
        }
    }
}
