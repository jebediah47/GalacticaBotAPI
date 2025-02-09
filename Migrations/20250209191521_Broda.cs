using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalacticaBotAPI.Migrations
{
    /// <inheritdoc />
    public partial class Broda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "AdminUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(
                    new DateTime(2025, 2, 9, 19, 15, 20, 901, DateTimeKind.Unspecified).AddTicks(
                        746
                    ),
                    new TimeSpan(0, 0, 0, 0, 0)
                )
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastUpdated", table: "AdminUsers");
        }
    }
}
