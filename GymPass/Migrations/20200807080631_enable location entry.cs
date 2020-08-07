using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class enablelocationentry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestLat",
                schema: "Identity",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestLong",
                schema: "Identity",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestLat",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TestLong",
                schema: "Identity",
                table: "User");
        }
    }
}
