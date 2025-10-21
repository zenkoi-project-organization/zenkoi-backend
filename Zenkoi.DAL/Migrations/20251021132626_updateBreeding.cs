using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateBreeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentSurvivalRate",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FertilizationRate",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEggs",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSurvivalRate",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.DropColumn(
                name: "FertilizationRate",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.DropColumn(
                name: "TotalEggs",
                schema: "dbo",
                table: "BreedingProcesses");
        }
    }
}
