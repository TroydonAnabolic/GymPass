using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class anupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "UserTrainingDuration",
                table: "Facility",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalTrainingDuration",
                table: "Facility",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddColumn<bool>(
                name: "WillUseCardioRoom",
                table: "Facility",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseStretchRoom",
                table: "Facility",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillUseWeightsRoom",
                table: "Facility",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WillUseCardioRoom",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "WillUseStretchRoom",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "WillUseWeightsRoom",
                table: "Facility");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "UserTrainingDuration",
                table: "Facility",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalTrainingDuration",
                table: "Facility",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);
        }
    }
}
