using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymPass.Migrations
{
    public partial class InitialCreate : Migration
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
                    NumberOfClientsInGym = table.Column<int>(nullable: false),
                    NumberOfClientsUsingWeightRoom = table.Column<int>(nullable: false),
                    NumberOfClientsUsingCardioRoom = table.Column<int>(nullable: false),
                    NumberOfClientsUsingStretchRoom = table.Column<int>(nullable: false),
                    IsOpenDoorRequested = table.Column<bool>(nullable: false),
                    DoorOpened = table.Column<bool>(nullable: false),
                    DoorCloseTimer = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.FacilityID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Facility");
        }
    }
}
