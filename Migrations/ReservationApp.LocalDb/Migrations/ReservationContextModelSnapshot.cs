﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReservationApp.Models;

#nullable disable

namespace ReservationApp.LocalDb.Migrations
{
    [DbContext(typeof(ReservationContext))]
    partial class ReservationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ReservationApp.Models.Database.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrderGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("OrderGuid");

                    b.ToTable("Reservations");

                    b.HasAnnotation("Relational:JsonPropertyName", "reservations");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.Order", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Id")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)")
                        .HasAnnotation("Relational:JsonPropertyName", "id");

                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Guid");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserGuid");

                    b.ToTable("Orders");

                    b.HasAnnotation("Relational:JsonPropertyName", "orders");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.ReservationTime", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ReservationGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Guid");

                    b.HasIndex("ReservationGuid");

                    b.ToTable("ReservationTimes");

                    b.HasAnnotation("Relational:JsonPropertyName", "times");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasAnnotation("Relational:JsonPropertyName", "name");

                    b.HasKey("Guid");

                    b.ToTable("Users");

                    b.HasAnnotation("Relational:JsonPropertyName", "user");
                });

            modelBuilder.Entity("ReservationApp.Models.Database.Entities.Reservation", b =>
                {
                    b.HasOne("ReservationApp.Models.Entities.Order", "Order")
                        .WithMany("Reservations")
                        .HasForeignKey("OrderGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.Order", b =>
                {
                    b.HasOne("ReservationApp.Models.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.ReservationTime", b =>
                {
                    b.HasOne("ReservationApp.Models.Database.Entities.Reservation", "Reservation")
                        .WithMany("Times")
                        .HasForeignKey("ReservationGuid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("ReservationApp.Models.Database.Entities.Reservation", b =>
                {
                    b.Navigation("Times");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.Order", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("ReservationApp.Models.Entities.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
