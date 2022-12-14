// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkPlan.Api.Brokers.Storages;

#nullable disable

namespace WorkPlan.Api.Migrations
{
    [DbContext(typeof(StorageBroker))]
    [Migration("20221011112102_AddPlan")]
    partial class AddPlan
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-preview.7.22376.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WorkPlan.Api.Models.Plans.Plan", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("ShiftID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WorkerID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ID");

                    b.HasIndex("ShiftID");

                    b.HasIndex("WorkerID");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("WorkPlan.Api.Models.Shifts.Shift", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("EndHour")
                        .HasColumnType("time");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("StartHour")
                        .HasColumnType("time");

                    b.HasKey("ID");

                    b.ToTable("Shifts");
                });

            modelBuilder.Entity("WorkPlan.Api.Models.Workers.Worker", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Workers");
                });

            modelBuilder.Entity("WorkPlan.Api.Models.Plans.Plan", b =>
                {
                    b.HasOne("WorkPlan.Api.Models.Shifts.Shift", "Shift")
                        .WithMany("Plans")
                        .HasForeignKey("ShiftID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("WorkPlan.Api.Models.Workers.Worker", "Worker")
                        .WithMany("Plans")
                        .HasForeignKey("WorkerID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Shift");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("WorkPlan.Api.Models.Shifts.Shift", b =>
                {
                    b.Navigation("Plans");
                });

            modelBuilder.Entity("WorkPlan.Api.Models.Workers.Worker", b =>
                {
                    b.Navigation("Plans");
                });
#pragma warning restore 612, 618
        }
    }
}
