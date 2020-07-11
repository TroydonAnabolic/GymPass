using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Data.Migrations
{
    public partial class UpdateLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExitGymRequest",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitGymRequest",
                schema: "Identity",
                table: "User");
        }
    }
}
