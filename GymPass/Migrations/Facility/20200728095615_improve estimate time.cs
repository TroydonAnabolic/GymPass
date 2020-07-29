using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class improveestimatetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedTimeToCheck",
                table: "UsersInGymDetails");

            migrationBuilder.CreateTable(
                name: "UsersOutofGymDetails",
                columns: table => new
                {
                    UsersOutOfGymDetailsID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityID = table.Column<int>(nullable: false),
                    EstimatedTimeToCheck = table.Column<DateTime>(nullable: false),
                    UniqueEntryID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersOutofGymDetails", x => x.UsersOutOfGymDetailsID);
                    table.ForeignKey(
                        name: "FK_UsersOutofGymDetails_Facility_FacilityID",
                        column: x => x.FacilityID,
                        principalTable: "Facility",
                        principalColumn: "FacilityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersOutofGymDetails_FacilityID",
                table: "UsersOutofGymDetails",
                column: "FacilityID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersOutofGymDetails");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedTimeToCheck",
                table: "UsersInGymDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
