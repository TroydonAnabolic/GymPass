using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class fixtypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWithin10m",
                table: "Facility",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWithin10m",
                table: "Facility");
        }
    }
}
