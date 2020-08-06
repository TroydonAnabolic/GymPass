using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class Initializedefaulttargetimg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                schema: "Identity",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "isVerifiedUser",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isVerifiedUser",
                schema: "Identity",
                table: "User");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                schema: "Identity",
                table: "User",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
