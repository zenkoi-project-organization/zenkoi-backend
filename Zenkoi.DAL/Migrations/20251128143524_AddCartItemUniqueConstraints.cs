using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId",
                schema: "dbo",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_KoiFishId",
                schema: "dbo",
                table: "CartItems",
                columns: new[] { "CartId", "KoiFishId" },
                unique: true,
                filter: "[KoiFishId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_PacketFishId",
                schema: "dbo",
                table: "CartItems",
                columns: new[] { "CartId", "PacketFishId" },
                unique: true,
                filter: "[PacketFishId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_KoiFishId",
                schema: "dbo",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_PacketFishId",
                schema: "dbo",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                schema: "dbo",
                table: "CartItems",
                column: "CartId");
        }
    }
}
