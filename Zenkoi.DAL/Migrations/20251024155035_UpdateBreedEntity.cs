using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBreedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentSurvivalRate",
                schema: "dbo",
                table: "BreedingProcesses",
                newName: "SurvivalRate");

            migrationBuilder.AddColumn<double>(
                name: "HatchingRate",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HatchingRate",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.RenameColumn(
                name: "SurvivalRate",
                schema: "dbo",
                table: "BreedingProcesses",
                newName: "CurrentSurvivalRate");
        }
    }
}
