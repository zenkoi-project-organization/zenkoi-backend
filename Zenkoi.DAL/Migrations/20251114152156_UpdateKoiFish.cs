using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKoiFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatternType",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AddColumn<string>(
                name: "Pattern",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pattern",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AddColumn<int>(
                name: "PatternType",
                schema: "dbo",
                table: "KoiFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
