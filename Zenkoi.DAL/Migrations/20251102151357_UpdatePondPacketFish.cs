using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePondPacketFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "QuantityPacket");

            migrationBuilder.AddColumn<int>(
                name: "QuantityFish",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityFish",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.RenameColumn(
                name: "QuantityPacket",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "Quantity");
        }
    }
}
