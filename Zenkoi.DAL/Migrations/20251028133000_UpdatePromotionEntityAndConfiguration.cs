using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePromotionEntityAndConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                schema: "dbo",
                table: "Promotions",
                newName: "DiscountValue");

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                schema: "dbo",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                schema: "dbo",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumOrderAmount",
                schema: "dbo",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                schema: "dbo",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                schema: "dbo",
                table: "Promotions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "MinimumOrderAmount",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "UsageLimit",
                schema: "dbo",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                schema: "dbo",
                table: "Promotions",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                schema: "dbo",
                table: "Promotions",
                type: "decimal(5,2)",
                nullable: true);
        }
    }
}
