using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateBreeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EggBatches_Ponds_PondId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FryFishes_Ponds_PondId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropColumn(
                name: "ImagesVideo",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                schema: "dbo",
                table: "PacketFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Video",
                schema: "dbo",
                table: "PacketFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "FryFishes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "EggBatches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EggBatches_Ponds_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FryFishes_Ponds_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EggBatches_Ponds_PondId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FryFishes_Ponds_PondId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropColumn(
                name: "Images",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.DropColumn(
                name: "Video",
                schema: "dbo",
                table: "PacketFishes");

            migrationBuilder.AddColumn<string>(
                name: "ImagesVideo",
                schema: "dbo",
                table: "PacketFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "FryFishes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "EggBatches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EggBatches_Ponds_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FryFishes_Ponds_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
