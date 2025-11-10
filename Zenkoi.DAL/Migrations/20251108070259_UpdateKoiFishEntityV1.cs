using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKoiFishEntityV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommonMutationType",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MutationRate",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommonMutationType",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.DropColumn(
                name: "MutationRate",
                schema: "dbo",
                table: "BreedingProcesses");
        }
    }
}
