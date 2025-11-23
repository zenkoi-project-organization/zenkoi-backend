using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class KoiBreeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MutationRate",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AddColumn<int>(
                name: "KoiBreedingStatus",
                schema: "dbo",
                table: "KoiFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KoiBreedingStatus",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AddColumn<double>(
                name: "MutationRate",
                schema: "dbo",
                table: "KoiFishes",
                type: "float",
                nullable: true);
        }
    }
}
