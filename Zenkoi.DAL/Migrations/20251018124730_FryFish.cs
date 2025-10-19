using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FryFish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "StageName",
                schema: "dbo",
                table: "ClassificationRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "FrySurvivalRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                schema: "dbo",
                table: "FrySurvivalRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                schema: "dbo",
                table: "FrySurvivalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "FryFishes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "dbo",
                table: "FryFishes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "FryFishes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalHatchedEggs",
                schema: "dbo",
                table: "EggBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "dbo",
                table: "ClassificationStages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "ClassificationStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                schema: "dbo",
                table: "ClassificationStages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "ClassificationStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StageNumber",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes",
                column: "BreedingProcessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "BreedingProcessId",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassificationStages_Ponds_PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropIndex(
                name: "IX_ClassificationStages_PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "FrySurvivalRecords");

            migrationBuilder.DropColumn(
                name: "Note",
                schema: "dbo",
                table: "FrySurvivalRecords");

            migrationBuilder.DropColumn(
                name: "Success",
                schema: "dbo",
                table: "FrySurvivalRecords");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropColumn(
                name: "TotalHatchedEggs",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "PondId",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "ClassificationStages");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                schema: "dbo",
                table: "ClassificationRecords");

            migrationBuilder.DropColumn(
                name: "StageNumber",
                schema: "dbo",
                table: "ClassificationRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "FryFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StageName",
                schema: "dbo",
                table: "ClassificationRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "BreedingProcessId");
        }
    }
}
