using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCountFromVarietyPacketFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                schema: "dbo",
                table: "VarietyPacketFishes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                schema: "dbo",
                table: "VarietyPacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
