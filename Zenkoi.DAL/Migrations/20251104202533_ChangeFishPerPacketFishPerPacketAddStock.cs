using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFishPerPacketFishPerPacketAddStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                schema: "dbo",
                table: "PacketFishes",
                newName: "PricePerPacket");

            migrationBuilder.AddColumn<int>(
                name: "FishPerPacket",
                schema: "dbo",
                table: "PacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                schema: "dbo",
                table: "PacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FishPerPacket",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.RenameColumn(
                name: "PricePerPacket",
                schema: "dbo",
                table: "PacketFishes",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "PacketFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
