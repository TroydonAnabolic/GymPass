using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class locationservices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EstimatedTrainingTime",
                table: "UsersInGymDetails",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "UserTrainingDuration",
                table: "Facility",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalTrainingDuration",
                table: "Facility",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCameraScanSuccessful",
                table: "Facility",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCameraScanSuccessful",
                table: "Facility");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EstimatedTrainingTime",
                table: "UsersInGymDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "UserTrainingDuration",
                table: "Facility",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalTrainingDuration",
                table: "Facility",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan));
        }
    }
}
