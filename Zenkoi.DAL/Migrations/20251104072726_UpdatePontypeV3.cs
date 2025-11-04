using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePontypeV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BelowMinLevel",
                schema: "dbo",
                table: "WaterParameterRecords",
                newName: "WaterLevelMeters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WaterLevelMeters",
                schema: "dbo",
                table: "WaterParameterRecords",
                newName: "BelowMinLevel");
        }
    }
}
