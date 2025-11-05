using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePondPacketFishSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_Ponds_PondId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropIndex(
                name: "IX_PondPacketFishes_PondId_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.RenameColumn(
                name: "QuantityPacket",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "SoldQuantity");

            migrationBuilder.RenameColumn(
                name: "QuantityFish",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "AvailableQuantity");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferReason",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferredAt",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferredToId",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "PondPacketFishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_PondId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "TransferredFromId",
                unique: true,
                filter: "[TransferredFromId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "BreedingProcessId",
                principalSchema: "dbo",
                principalTable: "BreedingProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PacketFishId",
                principalSchema: "dbo",
                principalTable: "PacketFishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_PondPacketFishes_TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "TransferredFromId",
                principalSchema: "dbo",
                principalTable: "PondPacketFishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_Ponds_PondId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_PondPacketFishes_TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_PondPacketFishes_Ponds_PondId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropIndex(
                name: "IX_PondPacketFishes_PondId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropIndex(
                name: "IX_PondPacketFishes_TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "TransferReason",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "TransferredAt",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "TransferredFromId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "TransferredToId",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "PondPacketFishes");

            migrationBuilder.RenameColumn(
                name: "SoldQuantity",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "QuantityPacket");

            migrationBuilder.RenameColumn(
                name: "AvailableQuantity",
                schema: "dbo",
                table: "PondPacketFishes",
                newName: "QuantityFish");

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_PondId_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                columns: new[] { "PondId", "PacketFishId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "BreedingProcessId",
                principalSchema: "dbo",
                principalTable: "BreedingProcesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PacketFishId",
                principalSchema: "dbo",
                principalTable: "PacketFishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PondPacketFishes_Ponds_PondId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
