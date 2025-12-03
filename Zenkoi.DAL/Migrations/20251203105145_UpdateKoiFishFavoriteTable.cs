using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKoiFishFavoriteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KoiFavorites_ApplicationUsers_UserId",
                schema: "dbo",
                table: "KoiFavorites");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "KoiFavorites",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_KoiFavorites_UserId_KoiFishId",
                schema: "dbo",
                table: "KoiFavorites",
                newName: "IX_KoiFavorites_CustomerId_KoiFishId");

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFavorites_Customers_CustomerId",
                schema: "dbo",
                table: "KoiFavorites",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KoiFavorites_Customers_CustomerId",
                schema: "dbo",
                table: "KoiFavorites");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "dbo",
                table: "KoiFavorites",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_KoiFavorites_CustomerId_KoiFishId",
                schema: "dbo",
                table: "KoiFavorites",
                newName: "IX_KoiFavorites_UserId_KoiFishId");

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFavorites_ApplicationUsers_UserId",
                schema: "dbo",
                table: "KoiFavorites",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
