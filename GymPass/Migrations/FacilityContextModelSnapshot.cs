﻿// <auto-generated />
using System;
using GymPass.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GymPass.Migrations
{
    [DbContext(typeof(FacilityContext))]
    partial class FacilityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GymPass.Models.Facility", b =>
                {
                    b.Property<int>("FacilityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<TimeSpan>("DoorCloseTimer")
                        .HasColumnType("time");

                    b.Property<bool>("DoorOpened")
                        .HasColumnType("bit");

                    b.Property<string>("FacilityName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOpenDoorRequested")
                        .HasColumnType("bit");

                    b.Property<int>("NumberOfClientsInGym")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfClientsUsingCardioRoom")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfClientsUsingStretchRoom")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfClientsUsingWeightRoom")
                        .HasColumnType("int");

                    b.HasKey("FacilityID");

                    b.ToTable("Facility");
                });
#pragma warning restore 612, 618
        }
    }
}
