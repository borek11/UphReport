﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UphReport.Data;

#nullable disable

namespace UphReport.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20230425091753_i4")]
    partial class i4
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedElement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ElementName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PageSpeedReportId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeOfResult")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PageSpeedReportId");

                    b.ToTable("PageSpeedElements");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("CreatedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("PSIVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Result")
                        .HasColumnType("real");

                    b.Property<int>("Strategy")
                        .HasColumnType("int");

                    b.Property<string>("SystemVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WebName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("WebPageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("WebPageId");

                    b.ToTable("PageSpeedReports");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedSubElement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PageSpeedElementId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Selector")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Snippet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PageSpeedElementId");

                    b.ToTable("PageSpeedSubElements");
                });

            modelBuilder.Entity("UphReport.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("UphReport.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UphReport.Entities.Wave.WaveAPIKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("APIKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CreditsRemaining")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("WaveAPIKeys");
                });

            modelBuilder.Entity("UphReport.Entities.WebPage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("DomainName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCheck")
                        .HasColumnType("bit");

                    b.Property<string>("WebName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WebPages");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedElement", b =>
                {
                    b.HasOne("UphReport.Entities.PageSpeedInsights.PageSpeedReport", "PageSpeedReport")
                        .WithMany("PageSpeedElement")
                        .HasForeignKey("PageSpeedReportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PageSpeedReport");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedReport", b =>
                {
                    b.HasOne("UphReport.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("UphReport.Entities.WebPage", null)
                        .WithMany("PageSpeedReport")
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedSubElement", b =>
                {
                    b.HasOne("UphReport.Entities.PageSpeedInsights.PageSpeedElement", "PageSpeedElement")
                        .WithMany("PageSpeedSubElement")
                        .HasForeignKey("PageSpeedElementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PageSpeedElement");
                });

            modelBuilder.Entity("UphReport.Entities.User", b =>
                {
                    b.HasOne("UphReport.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedElement", b =>
                {
                    b.Navigation("PageSpeedSubElement");
                });

            modelBuilder.Entity("UphReport.Entities.PageSpeedInsights.PageSpeedReport", b =>
                {
                    b.Navigation("PageSpeedElement");
                });

            modelBuilder.Entity("UphReport.Entities.WebPage", b =>
                {
                    b.Navigation("PageSpeedReport");
                });
#pragma warning restore 612, 618
        }
    }
}