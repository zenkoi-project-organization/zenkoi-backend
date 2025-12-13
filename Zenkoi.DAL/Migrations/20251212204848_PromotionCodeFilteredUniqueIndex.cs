using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PromotionCodeFilteredUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_Code",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Code",
                schema: "dbo",
                table: "Promotions",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promotions_Code",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Code",
                schema: "dbo",
                table: "Promotions",
                column: "Code",
                unique: true);
        }
    }
}
