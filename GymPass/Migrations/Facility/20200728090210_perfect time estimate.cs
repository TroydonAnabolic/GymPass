using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class perfecttimeestimate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedTimeToCheck",
                table: "UsersInGymDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedTimeToCheck",
                table: "UsersInGymDetails");
        }
    }
}
