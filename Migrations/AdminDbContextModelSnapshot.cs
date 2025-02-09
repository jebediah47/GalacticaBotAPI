﻿// <auto-generated />
using GalacticaBotAPI.Features.Admin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GalacticaBotAPI.Migrations
{
    [DbContext(typeof(AdminDbContext))]
    partial class AdminDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GalacticaBotAPI.Features.Admin.Models.AdminUser", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("AvatarHash")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Email")
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<bool>("IsProfileComplete")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("UserId");

                    b.ToTable("AdminUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
