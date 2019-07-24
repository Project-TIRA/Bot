﻿// <auto-generated />
using System;
using EntityModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityModel.Migrations
{
    [DbContext(typeof(DbModel))]
    [Migration("20190722192126_OrganizationAddress")]
    partial class OrganizationAddress
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EntityModel.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int>("AgeRangeEnd");

                    b.Property<int>("AgeRangeStart");

                    b.Property<string>("City");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("Gender");

                    b.Property<bool>("IsComplete");

                    b.Property<bool>("IsVerified");

                    b.Property<string>("Name");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("State");

                    b.Property<int>("TotalBeds");

                    b.Property<int>("UpdateFrequency");

                    b.Property<string>("Zip");

                    b.HasKey("Id");

                    b.HasIndex("PhoneNumber")
                        .IsUnique()
                        .HasFilter("[PhoneNumber] IS NOT NULL");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("EntityModel.Snapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<bool>("IsComplete");

                    b.Property<int>("OpenBeds");

                    b.Property<Guid>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Snapshots");
                });

            modelBuilder.Entity("EntityModel.Snapshot", b =>
                {
                    b.HasOne("EntityModel.Organization", "Organization")
                        .WithMany("Snapshots")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
