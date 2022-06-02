using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parlivote.Core.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("3b45d763-8154-410d-8e63-a07e19b6db5a"), "3b45d763-8154-410d-8e63-a07e19b6db5a", "chair", "CHAIR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("f1f532a2-7121-442c-a745-121e0e87173e"), "f1f532a2-7121-442c-a745-121e0e87173e", "parliamentarian", "PARLIAMENTARIAN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3b45d763-8154-410d-8e63-a07e19b6db5a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f1f532a2-7121-442c-a745-121e0e87173e"));
        }
    }
}
