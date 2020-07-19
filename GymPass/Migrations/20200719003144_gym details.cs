using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class gymdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersInGymDetails",
                columns: table => new
                {
                    UsersInGymDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    TimeAccessGranted = table.Column<DateTime>(nullable: false),
                    EstimatedTrainingTime = table.Column<TimeSpan>(nullable: false),
                    UniqueEntryID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInGymDetails", x => x.UsersInGymDetailID);
                    table.ForeignKey(
                        name: "FK_UsersInGymDetails_Facility_FacilityID",
                        column: x => x.FacilityID,
                        principalTable: "Facility",
                        principalColumn: "FacilityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersInGymDetails_FacilityID",
                table: "UsersInGymDetails",
                column: "FacilityID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersInGymDetails");
        }
    }
}
