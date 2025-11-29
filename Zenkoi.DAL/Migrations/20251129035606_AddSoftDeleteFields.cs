using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingDistances_IsActive",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropIndex(
                name: "IX_ShippingDistances_IsActive_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropIndex(
                name: "IX_ShippingBoxes_IsActive",
                schema: "dbo",
                table: "ShippingBoxes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "ShippingBoxes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "CustomerAddresses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Varieties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Varieties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Varieties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Varieties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "ShippingDistances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ShippingDistances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "ShippingBoxes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ShippingBoxes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "PondTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PondTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PondTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "PondTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Ponds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Ponds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Ponds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Patterns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Patterns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Patterns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Patterns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PacketFishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PacketFishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "KoiFishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "KoiFishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "IncidentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "IncidentTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "IncidentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "IncidentTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "CustomerAddresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "CustomerAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Areas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Areas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Areas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Areas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_IsDeleted",
                schema: "dbo",
                table: "ShippingDistances",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_IsDeleted_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances",
                columns: new[] { "IsDeleted", "MinDistanceKm", "MaxDistanceKm" });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxes_IsDeleted",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShippingDistances_IsDeleted",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropIndex(
                name: "IX_ShippingDistances_IsDeleted_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropIndex(
                name: "IX_ShippingBoxes_IsDeleted",
                schema: "dbo",
                table: "ShippingBoxes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Varieties");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ShippingDistances");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "ShippingBoxes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ShippingBoxes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "PondTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PondTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PondTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "PondTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Patterns");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Patterns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Patterns");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Patterns");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "IncidentTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "IncidentTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "IncidentTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "IncidentTypes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "Areas");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "ShippingDistances",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "ShippingBoxes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "CustomerAddresses",
                type: "bit",
                nullable: false,
                defaultValue: true);

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
                name: "IX_ShippingBoxes_IsActive",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "IsActive");
        }
    }
}
