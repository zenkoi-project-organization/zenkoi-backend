using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePondForBreed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassificationStages_Ponds_PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropIndex(
                name: "IX_ClassificationStages_PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "IncubationDailyRecords",
                newName: "UpdatedAt");

            // --- Fix lỗi: không thể ALTER int -> datetime2 ---
            migrationBuilder.DropColumn(
                name: "DayNumber",
                schema: "dbo",
                table: "IncubationDailyRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "DayNumber",
                schema: "dbo",
                table: "IncubationDailyRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "DayNumber",
                schema: "dbo",
                table: "FrySurvivalRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "DayNumber",
                schema: "dbo",
                table: "FrySurvivalRecords",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "IncubationDailyRecords",
                newName: "CreatedAt");

            // --- Hoàn tác thay đổi kiểu dữ liệu DayNumber ---
            migrationBuilder.DropColumn(
                name: "DayNumber",
                schema: "dbo",
                table: "IncubationDailyRecords");

            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                schema: "dbo",
                table: "IncubationDailyRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "DayNumber",
                schema: "dbo",
                table: "FrySurvivalRecords");

            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                schema: "dbo",
                table: "FrySurvivalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // --- Khôi phục lại PondId và foreign key ---
            migrationBuilder.AddColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "ClassificationStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationStages_PondId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "PondId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassificationStages_Ponds_PondId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
