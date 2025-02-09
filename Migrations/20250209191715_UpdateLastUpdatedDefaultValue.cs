using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalacticaBotAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastUpdatedDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "AdminUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(
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
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "AdminUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(
                    new DateTime(2025, 2, 9, 19, 15, 20, 901, DateTimeKind.Unspecified).AddTicks(
                        746
                    ),
                    new TimeSpan(0, 0, 0, 0, 0)
                ),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );
        }
    }
}
