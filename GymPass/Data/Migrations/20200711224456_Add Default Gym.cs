using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Data.Migrations
{
    public partial class AddDefaultGym : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultGym",
                schema: "Identity",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultGym",
                schema: "Identity",
                table: "User");
        }
    }
}
