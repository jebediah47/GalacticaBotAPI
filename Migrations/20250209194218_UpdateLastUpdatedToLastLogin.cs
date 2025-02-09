using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalacticaBotAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastUpdatedToLastLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "AdminUsers",
                newName: "LastLogin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "AdminUsers",
                newName: "LastUpdated");
        }
    }
}
