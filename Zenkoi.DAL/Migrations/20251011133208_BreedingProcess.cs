using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BreedingProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAreaSQM = table.Column<double>(type: "float", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PondTypes",
                schema: "dbo",
                columns: table => new
                {
                    PondTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedCapacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondTypes", x => x.PondTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Varieties",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarietyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Characteristic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginCountry = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varieties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ponds",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondTypeId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    PondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PondStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapacityLiters = table.Column<double>(type: "float", nullable: true),
                    DepthMeters = table.Column<double>(type: "float", nullable: true),
                    LengthMeters = table.Column<double>(type: "float", nullable: true),
                    WidthMeters = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ponds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ponds_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "dbo",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ponds_PondTypes_PondTypeId",
                        column: x => x.PondTypeId,
                        principalSchema: "dbo",
                        principalTable: "PondTypes",
                        principalColumn: "PondTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreedingProcesses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaleKoiId = table.Column<int>(type: "int", nullable: false),
                    FemaleKoiId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalFishQualified = table.Column<int>(type: "int", nullable: true),
                    TotalPackage = table.Column<int>(type: "int", nullable: true),
                    PondId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreedingProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreedingProcesses_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BreedingProcesses_Ponds_PondId1",
                        column: x => x.PondId1,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassificationStages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreedingProcessId = table.Column<int>(type: "int", nullable: false),
                    TotalCount = table.Column<int>(type: "int", nullable: true),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    QualifiedCount = table.Column<int>(type: "int", nullable: true),
                    UnqualifiedCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassificationStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassificationStages_BreedingProcesses_BreedingProcessId",
                        column: x => x.BreedingProcessId,
                        principalSchema: "dbo",
                        principalTable: "BreedingProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EggBatches",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreedingProcessId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    FertilizationRate = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HatchingTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpawnDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EggBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EggBatches_BreedingProcesses_BreedingProcessId",
                        column: x => x.BreedingProcessId,
                        principalSchema: "dbo",
                        principalTable: "BreedingProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EggBatches_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FryFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreedingProcessId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    InitialCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentSurvivalRate = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FryFishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FryFishes_BreedingProcesses_BreedingProcessId",
                        column: x => x.BreedingProcessId,
                        principalSchema: "dbo",
                        principalTable: "BreedingProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FryFishes_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KoiFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    BreedingProcessId = table.Column<int>(type: "int", nullable: true),
                    VarietyId = table.Column<int>(type: "int", nullable: false),
                    RFID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagesVideos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BodyShape = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiFishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiFishes_BreedingProcesses_BreedingProcessId",
                        column: x => x.BreedingProcessId,
                        principalSchema: "dbo",
                        principalTable: "BreedingProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_KoiFishes_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KoiFishes_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "dbo",
                        principalTable: "Varieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassificationRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassificationStageId = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    QualifiedCount = table.Column<int>(type: "int", nullable: true),
                    UnqualifiedCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassificationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassificationRecords_ClassificationStages_ClassificationStageId",
                        column: x => x.ClassificationStageId,
                        principalSchema: "dbo",
                        principalTable: "ClassificationStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncubationDailyRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EggBatchId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    HealthyEggs = table.Column<int>(type: "int", nullable: true),
                    RottenEggs = table.Column<int>(type: "int", nullable: true),
                    HatchedEggs = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncubationDailyRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncubationDailyRecords_EggBatches_EggBatchId",
                        column: x => x.EggBatchId,
                        principalSchema: "dbo",
                        principalTable: "EggBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FrySurvivalRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FryFishId = table.Column<int>(type: "int", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    SurvivalRate = table.Column<double>(type: "float", nullable: true),
                    CountAlive = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrySurvivalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrySurvivalRecords_FryFishes_FryFishId",
                        column: x => x.FryFishId,
                        principalSchema: "dbo",
                        principalTable: "FryFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_FemaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "FemaleKoiId");

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_MaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "MaleKoiId");

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_PondId",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_PondId1",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "PondId1",
                unique: true,
                filter: "[PondId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationRecords_ClassificationStageId",
                schema: "dbo",
                table: "ClassificationRecords",
                column: "ClassificationStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FrySurvivalRecords_FryFishId",
                schema: "dbo",
                table: "FrySurvivalRecords",
                column: "FryFishId");

            migrationBuilder.CreateIndex(
                name: "IX_IncubationDailyRecords_EggBatchId",
                schema: "dbo",
                table: "IncubationDailyRecords",
                column: "EggBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFishes_BreedingProcessId",
                schema: "dbo",
                table: "KoiFishes",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFishes_PondId",
                schema: "dbo",
                table: "KoiFishes",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFishes_VarietyId",
                schema: "dbo",
                table: "KoiFishes",
                column: "VarietyId");

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_AreaId",
                schema: "dbo",
                table: "Ponds",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_PondTypeId",
                schema: "dbo",
                table: "Ponds",
                column: "PondTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BreedingProcesses_KoiFishes_FemaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "FemaleKoiId",
                principalSchema: "dbo",
                principalTable: "KoiFishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BreedingProcesses_KoiFishes_MaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "MaleKoiId",
                principalSchema: "dbo",
                principalTable: "KoiFishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreedingProcesses_KoiFishes_FemaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.DropForeignKey(
                name: "FK_BreedingProcesses_KoiFishes_MaleKoiId",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.DropTable(
                name: "ClassificationRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FrySurvivalRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IncubationDailyRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ClassificationStages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FryFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "EggBatches",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "KoiFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BreedingProcesses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Varieties",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Ponds",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondTypes",
                schema: "dbo");
        }
    }
}
