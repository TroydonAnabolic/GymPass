using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class rekogfacedetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeRangeHigh",
                table: "UsersInGymDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AgeRangeLow",
                table: "UsersInGymDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "UsersInGymDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSmiling",
                table: "UsersInGymDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRangeHigh",
                table: "UsersInGymDetails");

            migrationBuilder.DropColumn(
                name: "AgeRangeLow",
                table: "UsersInGymDetails");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "UsersInGymDetails");

            migrationBuilder.DropColumn(
                name: "IsSmiling",
                table: "UsersInGymDetails");
        }
    }
}
