using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorIncidentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpactLevel",
                schema: "dbo",
                table: "PondIncidents");

            migrationBuilder.DropColumn(
                name: "ReportedAt",
                schema: "dbo",
                table: "PondIncidents");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                schema: "dbo",
                table: "PondIncidents");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "PondIncidents");

            migrationBuilder.DropColumn(
                name: "Severity",
                schema: "dbo",
                table: "KoiIncidents");

            migrationBuilder.RenameColumn(
                name: "SeverityLevel",
                schema: "dbo",
                table: "IncidentTypes",
                newName: "DefaultSeverity");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresWaterChange",
                schema: "dbo",
                table: "PondIncidents",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresQuarantine",
                schema: "dbo",
                table: "IncidentTypes",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AffectsBreeding",
                schema: "dbo",
                table: "IncidentTypes",
                type: "bit",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Incidents",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Incidents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Incidents");

            migrationBuilder.RenameColumn(
                name: "DefaultSeverity",
                schema: "dbo",
                table: "IncidentTypes",
                newName: "SeverityLevel");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresWaterChange",
                schema: "dbo",
                table: "PondIncidents",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImpactLevel",
                schema: "dbo",
                table: "PondIncidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportedAt",
                schema: "dbo",
                table: "PondIncidents",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                schema: "dbo",
                table: "PondIncidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "PondIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                schema: "dbo",
                table: "KoiIncidents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "RequiresQuarantine",
                schema: "dbo",
                table: "IncidentTypes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "AffectsBreeding",
                schema: "dbo",
                table: "IncidentTypes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: false);
        }
    }
}
