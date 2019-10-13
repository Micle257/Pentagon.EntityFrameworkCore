using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pentagon.EntityFrameworkCore.TestApp.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "User",
                nullable: false,
                defaultValueSql: "SYSDATETIMEOFFSET()");

            migrationBuilder.AddColumn<string>(
                name: "CreatedUser",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedUserId",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "User");
        }
    }
}
