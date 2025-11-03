using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingDistanceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingDistances",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinDistanceKm = table.Column<int>(type: "int", nullable: false),
                    MaxDistanceKm = table.Column<int>(type: "int", nullable: false),
                    PricePerKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingDistances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_IsActive",
                schema: "dbo",
                table: "ShippingDistances",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_IsActive_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances",
                columns: new[] { "IsActive", "MinDistanceKm", "MaxDistanceKm" });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances",
                columns: new[] { "MinDistanceKm", "MaxDistanceKm" });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_Name",
                schema: "dbo",
                table: "ShippingDistances",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingDistances",
                schema: "dbo");
        }
    }
}
