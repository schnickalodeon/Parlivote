using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class SeedApplicantRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9e1f495f-8bfb-4b21-ac71-d54679e0e3c1"), "9e1f495f-8bfb-4b21-ac71-d54679e0e3c1", "applicant", "APPLICANT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9e1f495f-8bfb-4b21-ac71-d54679e0e3c1"));
        }
    }
}
