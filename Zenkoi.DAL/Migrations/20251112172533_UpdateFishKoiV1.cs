using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFishKoiV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MutationType",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "CommonMutationType",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.AddColumn<string>(
                name: "MutationDescription",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MutationDescription",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AddColumn<int>(
                name: "MutationType",
                schema: "dbo",
                table: "KoiFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommonMutationType",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "int",
                nullable: true);
        }
    }
}
