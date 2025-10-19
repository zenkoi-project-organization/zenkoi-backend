using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateEggBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches",
                column: "BreedingProcessId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches",
                column: "BreedingProcessId");
        }
    }
}
