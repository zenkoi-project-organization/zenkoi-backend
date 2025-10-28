using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleStatusAndCartEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaleStatus",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NotForSale");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleStatus",
                schema: "dbo",
                table: "KoiFishes");
        }
    }
}
