using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations.Facility
{
    public partial class systemrestore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    FacilityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityName = table.Column<string>(nullable: true),
                    NumberOfClientsInGym = table.Column<int>(nullable: true),
                    NumberOfClientsUsingWeightRoom = table.Column<int>(nullable: true),
                    NumberOfClientsUsingCardioRoom = table.Column<int>(nullable: true),
                    NumberOfClientsUsingStretchRoom = table.Column<int>(nullable: true),
                    IsOpenDoorRequested = table.Column<bool>(nullable: false),
                    DoorOpened = table.Column<bool>(nullable: false),
                    DoorCloseTimer = table.Column<TimeSpan>(nullable: false),
                    UserTrainingDuration = table.Column<TimeSpan>(nullable: false),
                    TotalTrainingDuration = table.Column<TimeSpan>(nullable: false),
                    WillUseWeightsRoom = table.Column<bool>(nullable: false),
                    WillUseCardioRoom = table.Column<bool>(nullable: false),
                    WillUseStretchRoom = table.Column<bool>(nullable: false),
                    IsCameraScanSuccessful = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.FacilityID);
                });

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
                    UniqueEntryID = table.Column<string>(nullable: true)
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

            migrationBuilder.DropTable(
                name: "Facility");
        }
    }
}
