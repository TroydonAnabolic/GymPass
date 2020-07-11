using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Data.Migrations
{
    public partial class UpdatedCustomProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsingCardioRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsingStretchRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsingWeightsRoom",
                schema: "Identity",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
