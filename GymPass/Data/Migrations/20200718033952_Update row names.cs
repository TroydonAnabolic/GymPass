using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Data.Migrations
{
    public partial class Updaterownames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsingCardioRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsUsingStretchRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsUsingWeightsRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "HasLoggedWorkoutToday",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseCardioRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseStretchRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseWeightsRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLoggedWorkoutToday",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WillUseCardioRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WillUseStretchRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WillUseWeightsRoom",
                schema: "Identity",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsingCardioRoom",
                schema: "Identity",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsingStretchRoom",
                schema: "Identity",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsingWeightsRoom",
                schema: "Identity",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
