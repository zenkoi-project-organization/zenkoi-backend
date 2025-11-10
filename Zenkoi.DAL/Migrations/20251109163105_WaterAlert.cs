using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class WaterAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ParameterName",
                schema: "dbo",
                table: "WaterParameterThresholds",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "ParameterName",
                schema: "dbo",
                table: "WaterAlerts",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "AlertType",
                schema: "dbo",
                table: "WaterAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolveAt",
                schema: "dbo",
                table: "WaterAlerts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                schema: "dbo",
                table: "WaterAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterAlerts_WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts",
                column: "WaterParameterRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaterAlerts_WaterParameterRecords_WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts",
                column: "WaterParameterRecordId",
                principalSchema: "dbo",
                principalTable: "WaterParameterRecords",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaterAlerts_WaterParameterRecords_WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.DropIndex(
                name: "IX_WaterAlerts_WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.DropColumn(
                name: "AlertType",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.DropColumn(
                name: "ResolveAt",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.DropColumn(
                name: "Severity",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.DropColumn(
                name: "WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts");

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                schema: "dbo",
                table: "WaterParameterThresholds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ParameterName",
                schema: "dbo",
                table: "WaterAlerts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);
        }
    }
}
