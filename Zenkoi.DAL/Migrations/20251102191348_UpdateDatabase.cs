using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PondId1",
                schema: "dbo",
                table: "PondAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_PondId1",
                schema: "dbo",
                table: "PondAssignments",
                column: "PondId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PondAssignments_Ponds_PondId1",
                schema: "dbo",
                table: "PondAssignments",
                column: "PondId1",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondAssignments_Ponds_PondId1",
                schema: "dbo",
                table: "PondAssignments");

            migrationBuilder.DropIndex(
                name: "IX_PondAssignments_PondId1",
                schema: "dbo",
                table: "PondAssignments");

            migrationBuilder.DropColumn(
                name: "PondId1",
                schema: "dbo",
                table: "PondAssignments");
        }
    }
}
