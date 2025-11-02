using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PondPacketFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "BreedingProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "BreedingProcessId",
                principalSchema: "dbo",
                principalTable: "BreedingProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropIndex(
                name: "IX_PondPacketFishes_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes");
        }
    }
}
