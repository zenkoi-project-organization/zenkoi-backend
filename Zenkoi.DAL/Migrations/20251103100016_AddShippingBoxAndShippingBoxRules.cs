using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingBoxAndShippingBoxRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingBoxes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WeightCapacityLb = table.Column<int>(type: "int", nullable: false),
                    FeeUsd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxKoiCount = table.Column<int>(type: "int", nullable: true),
                    MaxKoiSizeInch = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingBoxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingBoxRules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingBoxId = table.Column<int>(type: "int", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: true),
                    MaxLengthCm = table.Column<int>(type: "int", nullable: true),
                    MinLengthCm = table.Column<int>(type: "int", nullable: true),
                    MaxWeightLb = table.Column<int>(type: "int", nullable: true),
                    ExtraInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingBoxRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingBoxRules_ShippingBoxes_ShippingBoxId",
                        column: x => x.ShippingBoxId,
                        principalSchema: "dbo",
                        principalTable: "ShippingBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxes_IsActive",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxes_Name",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_IsActive",
                schema: "dbo",
                table: "ShippingBoxRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_ShippingBoxId",
                schema: "dbo",
                table: "ShippingBoxRules",
                column: "ShippingBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_ShippingBoxId_Priority",
                schema: "dbo",
                table: "ShippingBoxRules",
                columns: new[] { "ShippingBoxId", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingBoxRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingBoxes",
                schema: "dbo");
        }
    }
}
