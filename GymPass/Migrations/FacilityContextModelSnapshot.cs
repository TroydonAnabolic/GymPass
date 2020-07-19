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

                    b.Property<int?>("NumberOfClientsInGym")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfClientsUsingCardioRoom")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfClientsUsingStretchRoom")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfClientsUsingWeightRoom")
                        .HasColumnType("int");

                    b.Property<TimeSpan?>("TotalTrainingDuration")
                        .HasColumnType("time");

                    b.Property<TimeSpan?>("UserTrainingDuration")
                        .HasColumnType("time");

                    b.Property<bool>("WillUseCardioRoom")
                        .HasColumnType("bit");

                    b.Property<bool>("WillUseStretchRoom")
                        .HasColumnType("bit");

                    b.Property<bool>("WillUseWeightsRoom")
                        .HasColumnType("bit");

                    b.HasKey("FacilityID");

                    b.ToTable("Facility");
                });

            modelBuilder.Entity("GymPass.Models.UsersInGymDetail", b =>
                {
                    b.Property<int>("UsersInGymDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<TimeSpan?>("EstimatedTrainingTime")
                        .HasColumnType("time");

                    b.Property<int>("FacilityID")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeAccessGranted")
                        .HasColumnType("datetime2");

                    b.Property<string>("UniqueEntryID")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UsersInGymDetailID");

                    b.HasIndex("FacilityID");

                    b.ToTable("UsersInGymDetails");
                });

            modelBuilder.Entity("GymPass.Models.UsersInGymDetail", b =>
                {
                    b.HasOne("GymPass.Models.Facility", "Facility")
                        .WithMany("UsersInGymDetails")
                        .HasForeignKey("FacilityID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
