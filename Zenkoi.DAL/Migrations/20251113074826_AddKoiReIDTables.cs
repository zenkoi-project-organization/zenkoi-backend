using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddKoiReIDTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KoiGalleryEnrollment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KoiFishId = table.Column<int>(type: "int", nullable: false),
                    FishIdInGallery = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumImages = table.Column<int>(type: "int", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnrolledBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiGalleryEnrollment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiGalleryEnrollment_ApplicationUsers_EnrolledBy",
                        column: x => x.EnrolledBy,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KoiGalleryEnrollment_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KoiIdentification",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KoiFishId = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdentifiedAs = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    IsUnknown = table.Column<bool>(type: "bit", nullable: false),
                    TopPredictions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiIdentification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiIdentification_ApplicationUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KoiIdentification_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KoiGalleryEnrollment_EnrolledBy",
                schema: "dbo",
                table: "KoiGalleryEnrollment",
                column: "EnrolledBy");

            migrationBuilder.CreateIndex(
                name: "IX_KoiGalleryEnrollment_FishIdInGallery",
                schema: "dbo",
                table: "KoiGalleryEnrollment",
                column: "FishIdInGallery");

            migrationBuilder.CreateIndex(
                name: "IX_KoiGalleryEnrollment_KoiFishId_IsActive",
                schema: "dbo",
                table: "KoiGalleryEnrollment",
                columns: new[] { "KoiFishId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_KoiIdentification_CreatedAt",
                schema: "dbo",
                table: "KoiIdentification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_KoiIdentification_CreatedBy",
                schema: "dbo",
                table: "KoiIdentification",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KoiIdentification_IsUnknown",
                schema: "dbo",
                table: "KoiIdentification",
                column: "IsUnknown");

            migrationBuilder.CreateIndex(
                name: "IX_KoiIdentification_KoiFishId",
                schema: "dbo",
                table: "KoiIdentification",
                column: "KoiFishId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KoiGalleryEnrollment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "KoiIdentification",
                schema: "dbo");
        }
    }
}
