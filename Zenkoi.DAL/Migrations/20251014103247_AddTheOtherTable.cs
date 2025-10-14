using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTheOtherTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KoiFishes_Ponds_PondId",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_KoiFishes_Varieties_VarietyId",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ponds_Areas_AreaId",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropForeignKey(
                name: "FK_Ponds_PondTypes_PondTypeId",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.DropIndex(
                name: "IX_BreedingProcesses_PondId1",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.RenameColumn(
                name: "PondTypeID",
                schema: "dbo",
                table: "PondTypes",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "VarietyName",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginCountry",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Characteristic",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "dbo",
                table: "PondTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "PondTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WidthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PondStatus",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PondName",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LengthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DepthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Ponds",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "CapacityLiters",
                schema: "dbo",
                table: "Ponds",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Size",
                schema: "dbo",
                table: "KoiFishes",
                type: "decimal(8,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RFID",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagesVideos",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HealthStatus",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "KoiFishes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "BodyShape",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAreaSQM",
                schema: "dbo",
                table: "Areas",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Areas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AreaName",
                schema: "dbo",
                table: "Areas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TotalOrders = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalSpent = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeverityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiresQuarantine = table.Column<bool>(type: "bit", nullable: true),
                    AffectsBreeding = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PacketFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Size = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    AgeMonths = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    ImagesVideo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacketFishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PondId = table.Column<int>(type: "int", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_ApplicationUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterAlerts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MeasuredValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolvedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterAlerts_ApplicationUsers_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WaterAlerts_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterParameterRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    PHLevel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TemperatureCelsius = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    OxygenLevel = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    AmmoniaLevel = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    NitriteLevel = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    NitrateLevel = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    Turbidity = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    TotalChlorines = table.Column<decimal>(type: "decimal(8,4)", nullable: true),
                    CarbonHardness = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    WaterLevelMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RecordedByUserId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterParameterRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterParameterRecords_ApplicationUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WaterParameterRecords_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterParameterThresholds",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PondTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterParameterThresholds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterParameterThresholds_PondTypes_PondTypeId",
                        column: x => x.PondTypeId,
                        principalSchema: "dbo",
                        principalTable: "PondTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentTypeId = table.Column<int>(type: "int", nullable: false),
                    IncidentTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportedByUserId = table.Column<int>(type: "int", nullable: false),
                    ResolvedByUserId = table.Column<int>(type: "int", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidents_ApplicationUsers_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_ApplicationUsers_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidents_IncidentTypes_IncidentTypeId",
                        column: x => x.IncidentTypeId,
                        principalSchema: "dbo",
                        principalTable: "IncidentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PondPacketFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    PacketFishId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondPacketFishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                        column: x => x.PacketFishId,
                        principalSchema: "dbo",
                        principalTable: "PacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VarietyPacketFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarietyId = table.Column<int>(type: "int", nullable: false),
                    PacketFishId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyPacketFishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VarietyPacketFishes_PacketFishes_PacketFishId",
                        column: x => x.PacketFishId,
                        principalSchema: "dbo",
                        principalTable: "PacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VarietyPacketFishes_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "dbo",
                        principalTable: "Varieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalSchema: "dbo",
                        principalTable: "Promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    TaskTemplateId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true, defaultValue: new TimeSpan(0, 0, 0, 0, 0)),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedOutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManagerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSchedules_ApplicationUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkSchedules_ApplicationUsers_StaffId",
                        column: x => x.StaffId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkSchedules_TaskTemplates_TaskTemplateId",
                        column: x => x.TaskTemplateId,
                        principalSchema: "dbo",
                        principalTable: "TaskTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KoiIncidents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    KoiFishId = table.Column<int>(type: "int", nullable: false),
                    AffectedStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecificSymptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresTreatment = table.Column<bool>(type: "bit", nullable: false),
                    IsIsolated = table.Column<bool>(type: "bit", nullable: false),
                    AffectedFrom = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RecoveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TreatmentNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiIncidents_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalSchema: "dbo",
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KoiIncidents_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PondIncidents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    ImpactLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnvironmentalChanges = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiresWaterChange = table.Column<bool>(type: "bit", nullable: false),
                    FishDiedCount = table.Column<int>(type: "int", nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PondIncidents_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalSchema: "dbo",
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PondIncidents_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    KoiFishId = table.Column<int>(type: "int", nullable: true),
                    PacketFishId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.CheckConstraint("CK_OrderDetail_KoiOrPacket", "(KoiFishId IS NOT NULL AND PacketFishId IS NULL) OR (KoiFishId IS NULL AND PacketFishId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrderDetails_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_PacketFishes_PacketFishId",
                        column: x => x.PacketFishId,
                        principalSchema: "dbo",
                        principalTable: "PacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Gateway = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    PaymentInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_VarietyName",
                schema: "dbo",
                table: "Varieties",
                column: "VarietyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PondTypes_TypeName",
                schema: "dbo",
                table: "PondTypes",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_PondName",
                schema: "dbo",
                table: "Ponds",
                column: "PondName");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFishes_RFID",
                schema: "dbo",
                table: "KoiFishes",
                column: "RFID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_PondId1",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "PondId1");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApplicationUserId",
                schema: "dbo",
                table: "Customers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_IncidentTypeId",
                schema: "dbo",
                table: "Incidents",
                column: "IncidentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_OccurredAt",
                schema: "dbo",
                table: "Incidents",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ReportedByUserId",
                schema: "dbo",
                table: "Incidents",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_ResolvedByUserId",
                schema: "dbo",
                table: "Incidents",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentTypes_Name",
                schema: "dbo",
                table: "IncidentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KoiIncidents_IncidentId",
                schema: "dbo",
                table: "KoiIncidents",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiIncidents_KoiFishId",
                schema: "dbo",
                table: "KoiIncidents",
                column: "KoiFishId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_KoiFishId",
                schema: "dbo",
                table: "OrderDetails",
                column: "KoiFishId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                schema: "dbo",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_PacketFishId",
                schema: "dbo",
                table: "OrderDetails",
                column: "PacketFishId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                schema: "dbo",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                schema: "dbo",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                schema: "dbo",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromotionId",
                schema: "dbo",
                table: "Orders",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PacketFishes_Name",
                schema: "dbo",
                table: "PacketFishes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                schema: "dbo",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                schema: "dbo",
                table: "Payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                schema: "dbo",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PondIncidents_IncidentId",
                schema: "dbo",
                table: "PondIncidents",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_PondIncidents_PondId",
                schema: "dbo",
                table: "PondIncidents",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PacketFishId");

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_PondId_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                columns: new[] { "PondId", "PacketFishId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Code",
                schema: "dbo",
                table: "Promotions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_ValidFrom",
                schema: "dbo",
                table: "Promotions",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_ValidTo",
                schema: "dbo",
                table: "Promotions",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_AssignedToUserId",
                schema: "dbo",
                table: "TaskTemplates",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_PondId",
                schema: "dbo",
                table: "TaskTemplates",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_ScheduledAt",
                schema: "dbo",
                table: "TaskTemplates",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyPacketFishes_PacketFishId",
                schema: "dbo",
                table: "VarietyPacketFishes",
                column: "PacketFishId");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyPacketFishes_VarietyId_PacketFishId",
                schema: "dbo",
                table: "VarietyPacketFishes",
                columns: new[] { "VarietyId", "PacketFishId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterAlerts_CreatedAt",
                schema: "dbo",
                table: "WaterAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WaterAlerts_IsResolved",
                schema: "dbo",
                table: "WaterAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_WaterAlerts_PondId",
                schema: "dbo",
                table: "WaterAlerts",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterAlerts_ResolvedByUserId",
                schema: "dbo",
                table: "WaterAlerts",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterRecords_PondId",
                schema: "dbo",
                table: "WaterParameterRecords",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterRecords_RecordedAt",
                schema: "dbo",
                table: "WaterParameterRecords",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterRecords_RecordedByUserId",
                schema: "dbo",
                table: "WaterParameterRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterThresholds_ParameterName",
                schema: "dbo",
                table: "WaterParameterThresholds",
                column: "ParameterName");

            migrationBuilder.CreateIndex(
                name: "IX_WaterParameterThresholds_PondTypeId",
                schema: "dbo",
                table: "WaterParameterThresholds",
                column: "PondTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_StaffId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "TaskTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_WorkDate",
                schema: "dbo",
                table: "WorkSchedules",
                column: "WorkDate");

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFishes_Ponds_PondId",
                schema: "dbo",
                table: "KoiFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFishes_Varieties_VarietyId",
                schema: "dbo",
                table: "KoiFishes",
                column: "VarietyId",
                principalSchema: "dbo",
                principalTable: "Varieties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ponds_Areas_AreaId",
                schema: "dbo",
                table: "Ponds",
                column: "AreaId",
                principalSchema: "dbo",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ponds_PondTypes_PondTypeId",
                schema: "dbo",
                table: "Ponds",
                column: "PondTypeId",
                principalSchema: "dbo",
                principalTable: "PondTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KoiFishes_Ponds_PondId",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_KoiFishes_Varieties_VarietyId",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ponds_Areas_AreaId",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropForeignKey(
                name: "FK_Ponds_PondTypes_PondTypeId",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropTable(
                name: "KoiIncidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrderDetails",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondIncidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondPacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VarietyPacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterAlerts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterParameterRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterParameterThresholds",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WorkSchedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Incidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TaskTemplates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Promotions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IncidentTypes",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Varieties_VarietyName",
                schema: "dbo",
                table: "Varieties");

            migrationBuilder.DropIndex(
                name: "IX_PondTypes_TypeName",
                schema: "dbo",
                table: "PondTypes");

            migrationBuilder.DropIndex(
                name: "IX_Ponds_PondName",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropIndex(
                name: "IX_KoiFishes_RFID",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes");

            migrationBuilder.DropIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches");

            migrationBuilder.DropIndex(
                name: "IX_BreedingProcesses_PondId1",
                schema: "dbo",
                table: "BreedingProcesses");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "dbo",
                table: "PondTypes",
                newName: "PondTypeID");

            migrationBuilder.AlterColumn<string>(
                name: "VarietyName",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "OriginCountry",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Characteristic",
                schema: "dbo",
                table: "Varieties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                schema: "dbo",
                table: "PondTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "PondTypes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<double>(
                name: "WidthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PondStatus",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PondName",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "dbo",
                table: "Ponds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<double>(
                name: "LengthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DepthMeters",
                schema: "dbo",
                table: "Ponds",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "Ponds",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<double>(
                name: "CapacityLiters",
                schema: "dbo",
                table: "Ponds",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Size",
                schema: "dbo",
                table: "KoiFishes",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RFID",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ImagesVideos",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "HealthStatus",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "KoiFishes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "BodyShape",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "dbo",
                table: "BreedingProcesses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<double>(
                name: "TotalAreaSQM",
                schema: "dbo",
                table: "Areas",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "AreaName",
                schema: "dbo",
                table: "Areas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BreedingProcesses_PondId1",
                schema: "dbo",
                table: "BreedingProcesses",
                column: "PondId1",
                unique: true,
                filter: "[PondId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFishes_Ponds_PondId",
                schema: "dbo",
                table: "KoiFishes",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KoiFishes_Varieties_VarietyId",
                schema: "dbo",
                table: "KoiFishes",
                column: "VarietyId",
                principalSchema: "dbo",
                principalTable: "Varieties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ponds_Areas_AreaId",
                schema: "dbo",
                table: "Ponds",
                column: "AreaId",
                principalSchema: "dbo",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ponds_PondTypes_PondTypeId",
                schema: "dbo",
                table: "Ponds",
                column: "PondTypeId",
                principalSchema: "dbo",
                principalTable: "PondTypes",
                principalColumn: "PondTypeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
