using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePontypeV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "dbo",
                table: "WaterParameterThresholds");

            migrationBuilder.DropColumn(
                name: "TotalChlorines",
                schema: "dbo",
                table: "WaterParameterRecords");

            migrationBuilder.DropColumn(
                name: "Turbidity",
                schema: "dbo",
                table: "WaterParameterRecords");

            migrationBuilder.RenameColumn(
                name: "WaterLevelMeters",
                schema: "dbo",
                table: "WaterParameterRecords",
                newName: "BelowMinLevel");

            migrationBuilder.RenameColumn(
                name: "RecommendedCapacity",
                schema: "dbo",
                table: "PondTypes",
                newName: "RecommendedQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BelowMinLevel",
                schema: "dbo",
                table: "WaterParameterRecords",
                newName: "WaterLevelMeters");

            migrationBuilder.RenameColumn(
                name: "RecommendedQuantity",
                schema: "dbo",
                table: "PondTypes",
                newName: "RecommendedCapacity");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "dbo",
                table: "WaterParameterThresholds",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalChlorines",
                schema: "dbo",
                table: "WaterParameterRecords",
                type: "decimal(8,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Turbidity",
                schema: "dbo",
                table: "WaterParameterRecords",
                type: "decimal(8,2)",
                nullable: true);
        }
    }
}
