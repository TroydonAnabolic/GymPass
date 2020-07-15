using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Data.Migrations
{
    public partial class AccessGrantedTimeLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeAccessGranted",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeAccessGranted",
                schema: "Identity",
                table: "User");
        }
    }
}
