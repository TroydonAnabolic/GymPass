using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class databasecoords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Facility",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Facility",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Facility");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Facility");
        }
    }
}
