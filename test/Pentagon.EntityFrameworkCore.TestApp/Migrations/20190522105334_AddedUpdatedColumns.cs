using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pentagon.EntityFrameworkCore.TestApp.Migrations
{
    public partial class AddedUpdatedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedUser",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedUserId",
                table: "User",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedUser",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedUserId",
                table: "User");
        }
    }
}
