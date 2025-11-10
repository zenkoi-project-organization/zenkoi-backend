using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePacketFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.AddColumn<double>(
                name: "MaxSize",
                schema: "dbo",
                table: "PacketFishes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MinSize",
                schema: "dbo",
                table: "PacketFishes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSize",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.DropColumn(
                name: "MinSize",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.AddColumn<decimal>(
                name: "Size",
                schema: "dbo",
                table: "PacketFishes",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
