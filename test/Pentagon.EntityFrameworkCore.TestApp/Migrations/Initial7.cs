using Microsoft.EntityFrameworkCore.Migrations;

namespace Pentagon.EntityFrameworkCore.TestApp.Migrations
{
    public partial class Initial7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "a1",
                table: "User",
                nullable: true,
                defaultValue: 6);

            migrationBuilder.AddColumn<double>(
                name: "a2",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "a3",
                table: "User",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "a1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "a2",
                table: "User");

            migrationBuilder.DropColumn(
                name: "a3",
                table: "User");
        }
    }
}
