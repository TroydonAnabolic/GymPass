using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class addworkoutlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLoggedWorkoutToday",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "WillUseCardioRoom",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "WillUseStretchRoom",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "WillUseWeightsRoom",
                table: "Facility");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLoggedWorkoutToday",
                table: "Facility",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseCardioRoom",
                table: "Facility",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseStretchRoom",
                table: "Facility",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseWeightsRoom",
                table: "Facility",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
