using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ExpoPushToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TotalAreaSQM = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
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
                    DefaultSeverity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiresQuarantine = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    AffectsBreeding = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
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
                    FishPerPacket = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    PricePerPacket = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinSize = table.Column<double>(type: "float", nullable: false),
                    MaxSize = table.Column<double>(type: "float", nullable: false),
                    AgeMonths = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Videos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacketFishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patterns",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatternName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PondTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecommendedQuantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondTypes", x => x.Id);
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
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Images = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingBoxes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WeightCapacityLb = table.Column<int>(type: "int", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxKoiCount = table.Column<int>(type: "int", nullable: true),
                    MaxKoiSizeInch = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingBoxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingDistances",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinDistanceKm = table.Column<int>(type: "int", nullable: false),
                    MaxDistanceKm = table.Column<int>(type: "int", nullable: false),
                    PricePerKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaseFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingDistances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultDuration = table.Column<int>(type: "int", nullable: false),
                    NotesTask = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Varieties",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarietyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Characteristic = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OriginCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varieties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyScheduleTemplates",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyScheduleTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_Customers_ApplicationUsers_Id",
                        column: x => x.Id,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AvatarURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetails_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                schema: "dbo",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
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
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                name: "Ponds",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondTypeId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    PondName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PondStatus = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CapacityLiters = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CurrentCapacity = table.Column<double>(type: "float", nullable: true),
                    DepthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LengthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    WidthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    MaxFishCount = table.Column<int>(type: "int", nullable: true),
                    CurrentCount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ponds_PondTypes_PondTypeId",
                        column: x => x.PondTypeId,
                        principalSchema: "dbo",
                        principalTable: "PondTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterParameterThresholds",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterName = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
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
                name: "AspNetRoleClaims",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingBoxRules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingBoxId = table.Column<int>(type: "int", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxCount = table.Column<int>(type: "int", nullable: true),
                    MaxLengthCm = table.Column<int>(type: "int", nullable: true),
                    MinLengthCm = table.Column<int>(type: "int", nullable: true),
                    MaxWeightLb = table.Column<int>(type: "int", nullable: true),
                    ExtraInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingBoxRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingBoxRules_ShippingBoxes_ShippingBoxId",
                        column: x => x.ShippingBoxId,
                        principalSchema: "dbo",
                        principalTable: "ShippingBoxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskTemplateId = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Pending"),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSchedules_ApplicationUsers_CreatedBy",
                        column: x => x.CreatedBy,
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VarietyPacketFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarietyId = table.Column<int>(type: "int", nullable: false),
                    PacketFishId = table.Column<int>(type: "int", nullable: false)
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
                name: "VarietyPatterns",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatternId = table.Column<int>(type: "int", nullable: false),
                    VarietyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VarietyPatterns_Patterns_PatternId",
                        column: x => x.PatternId,
                        principalSchema: "dbo",
                        principalTable: "Patterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VarietyPatterns_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "dbo",
                        principalTable: "Varieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyScheduleTemplateItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeeklyScheduleTemplateId = table.Column<int>(type: "int", nullable: false),
                    TaskTemplateId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyScheduleTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyScheduleTemplateItems_TaskTemplates_TaskTemplateId",
                        column: x => x.TaskTemplateId,
                        principalSchema: "dbo",
                        principalTable: "TaskTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeeklyScheduleTemplateItems_WeeklyScheduleTemplates_WeeklyScheduleTemplateId",
                        column: x => x.WeeklyScheduleTemplateId,
                        principalSchema: "dbo",
                        principalTable: "WeeklyScheduleTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    DistanceFromFarmKm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DistanceCalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "dbo",
                        principalTable: "Customers",
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
                    PondId = table.Column<int>(type: "int", nullable: false),
                    EnvironmentalChanges = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiresWaterChange = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FishDiedCount = table.Column<int>(type: "int", nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                name: "PondAssignments",
                schema: "dbo",
                columns: table => new
                {
                    WorkScheduleId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    PondId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondAssignments", x => new { x.WorkScheduleId, x.PondId });
                    table.ForeignKey(
                        name: "FK_PondAssignments_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PondAssignments_Ponds_PondId1",
                        column: x => x.PondId1,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PondAssignments_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalSchema: "dbo",
                        principalTable: "WorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffAssignments",
                schema: "dbo",
                columns: table => new
                {
                    WorkScheduleId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    CompletionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAssignments", x => new { x.WorkScheduleId, x.StaffId });
                    table.ForeignKey(
                        name: "FK_StaffAssignments_ApplicationUsers_StaffId",
                        column: x => x.StaffId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffAssignments_WorkSchedules_WorkScheduleId",
                        column: x => x.WorkScheduleId,
                        principalSchema: "dbo",
                        principalTable: "WorkSchedules",
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
                    CustomerAddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_Orders_CustomerAddresses_CustomerAddressId",
                        column: x => x.CustomerAddressId,
                        principalSchema: "dbo",
                        principalTable: "CustomerAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "WaterAlerts",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    ParameterName = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    MeasuredValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    AlertType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ResolveAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolvedByUserId = table.Column<int>(type: "int", nullable: true),
                    WaterParameterRecordId = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_WaterAlerts_WaterParameterRecords_WaterParameterRecordId",
                        column: x => x.WaterParameterRecordId,
                        principalSchema: "dbo",
                        principalTable: "WaterParameterRecords",
                        principalColumn: "Id");
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

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActualOrderId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    PaymentUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ResponseData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Orders_ActualOrderId",
                        column: x => x.ActualOrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BreedingProcesses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaleKoiId = table.Column<int>(type: "int", nullable: false),
                    FemaleKoiId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
                    TotalEggs = table.Column<int>(type: "int", nullable: true),
                    FertilizationRate = table.Column<double>(type: "float", nullable: true),
                    HatchingRate = table.Column<double>(type: "float", nullable: true),
                    SurvivalRate = table.Column<double>(type: "float", nullable: true),
                    TotalFishQualified = table.Column<int>(type: "int", nullable: true),
                    TotalPackage = table.Column<int>(type: "int", nullable: true),
                    MutationRate = table.Column<double>(type: "float", nullable: true),
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
                    TotalCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    ShowQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    PondQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    CullQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    TotalHatchedEggs = table.Column<int>(type: "int", nullable: true),
                    FertilizationRate = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HatchingTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpawnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PondId = table.Column<int>(type: "int", nullable: true)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FryFishes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreedingProcessId = table.Column<int>(type: "int", nullable: false),
                    InitialCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentSurvivalRate = table.Column<double>(type: "float", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PondId = table.Column<int>(type: "int", nullable: true)
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
                        principalColumn: "Id");
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
                    RFID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaleStatus = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "NotForSale"),
                    KoiBreedingStatus = table.Column<int>(type: "int", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Videos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsMutated = table.Column<bool>(type: "bit", nullable: false),
                    MutationDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Origin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KoiFishes_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "dbo",
                        principalTable: "Varieties",
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
                    BreedingProcessId = table.Column<int>(type: "int", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "int", nullable: false),
                    SoldQuantity = table.Column<int>(type: "int", nullable: false),
                    TransferredFromId = table.Column<int>(type: "int", nullable: true),
                    TransferredToId = table.Column<int>(type: "int", nullable: true),
                    TransferredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransferReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondPacketFishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_BreedingProcesses_BreedingProcessId",
                        column: x => x.BreedingProcessId,
                        principalSchema: "dbo",
                        principalTable: "BreedingProcesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_PacketFishes_PacketFishId",
                        column: x => x.PacketFishId,
                        principalSchema: "dbo",
                        principalTable: "PacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_PondPacketFishes_TransferredFromId",
                        column: x => x.TransferredFromId,
                        principalSchema: "dbo",
                        principalTable: "PondPacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PondPacketFishes_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassificationRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassificationStageId = table.Column<int>(type: "int", nullable: false),
                    StageNumber = table.Column<int>(type: "int", nullable: false),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    ShowQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    PondQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    CullQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    DayNumber = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HealthyEggs = table.Column<int>(type: "int", nullable: true),
                    RottenEggs = table.Column<int>(type: "int", nullable: true),
                    HatchedEggs = table.Column<int>(type: "int", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    DayNumber = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SurvivalRate = table.Column<double>(type: "float", nullable: true),
                    CountAlive = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    KoiFishId = table.Column<int>(type: "int", nullable: true),
                    PacketFishId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalSchema: "dbo",
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_PacketFishes_PacketFishId",
                        column: x => x.PacketFishId,
                        principalSchema: "dbo",
                        principalTable: "PacketFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KoiFavorites",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    KoiFishId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiFavorites_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KoiFavorites_KoiFishes_KoiFishId",
                        column: x => x.KoiFishId,
                        principalSchema: "dbo",
                        principalTable: "KoiFishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "ApplicationUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "ApplicationUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "dbo",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "dbo",
                table: "AspNetUserClaims",
                column: "UserId");

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
                column: "PondId1");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                schema: "dbo",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_KoiFishId",
                schema: "dbo",
                table: "CartItems",
                column: "KoiFishId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PacketFishId",
                schema: "dbo",
                table: "CartItems",
                column: "PacketFishId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                schema: "dbo",
                table: "Carts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationRecords_ClassificationStageId",
                schema: "dbo",
                table: "ClassificationRecords",
                column: "ClassificationStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassificationStages_BreedingProcessId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "BreedingProcessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                schema: "dbo",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_BreedingProcessId",
                schema: "dbo",
                table: "EggBatches",
                column: "BreedingProcessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EggBatches_PondId",
                schema: "dbo",
                table: "EggBatches",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_BreedingProcessId",
                schema: "dbo",
                table: "FryFishes",
                column: "BreedingProcessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FryFishes_PondId",
                schema: "dbo",
                table: "FryFishes",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_FrySurvivalRecords_FryFishId",
                schema: "dbo",
                table: "FrySurvivalRecords",
                column: "FryFishId");

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
                name: "IX_IncubationDailyRecords_EggBatchId",
                schema: "dbo",
                table: "IncubationDailyRecords",
                column: "EggBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFavorites_KoiFishId",
                schema: "dbo",
                table: "KoiFavorites",
                column: "KoiFishId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiFavorites_UserId_KoiFishId",
                schema: "dbo",
                table: "KoiFavorites",
                columns: new[] { "UserId", "KoiFishId" },
                unique: true);

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
                name: "IX_KoiFishes_RFID",
                schema: "dbo",
                table: "KoiFishes",
                column: "RFID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KoiFishes_VarietyId",
                schema: "dbo",
                table: "KoiFishes",
                column: "VarietyId");

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
                name: "IX_Orders_CustomerAddressId",
                schema: "dbo",
                table: "Orders",
                column: "CustomerAddressId");

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
                name: "IX_Orders_Status_UpdatedAt",
                schema: "dbo",
                table: "Orders",
                columns: new[] { "Status", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UpdatedAt",
                schema: "dbo",
                table: "Orders",
                column: "UpdatedAt");

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
                name: "IX_PaymentTransactions_ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "ActualOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TransactionId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_UserId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_PondId",
                schema: "dbo",
                table: "PondAssignments",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_PondId1",
                schema: "dbo",
                table: "PondAssignments",
                column: "PondId1");

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_WorkScheduleId",
                schema: "dbo",
                table: "PondAssignments",
                column: "WorkScheduleId");

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
                name: "IX_PondPacketFishes_BreedingProcessId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "BreedingProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_PondPacketFishes_PacketFishId",
                schema: "dbo",
                table: "PondPacketFishes",
                column: "PacketFishId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_AreaId",
                schema: "dbo",
                table: "Ponds",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_PondName",
                schema: "dbo",
                table: "Ponds",
                column: "PondName");

            migrationBuilder.CreateIndex(
                name: "IX_Ponds_PondTypeId",
                schema: "dbo",
                table: "Ponds",
                column: "PondTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PondTypes_TypeName",
                schema: "dbo",
                table: "PondTypes",
                column: "TypeName",
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
                name: "IX_RefreshToken_UserId",
                schema: "dbo",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "dbo",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxes_IsActive",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxes_Name",
                schema: "dbo",
                table: "ShippingBoxes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_IsActive",
                schema: "dbo",
                table: "ShippingBoxRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_ShippingBoxId",
                schema: "dbo",
                table: "ShippingBoxRules",
                column: "ShippingBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingBoxRules_ShippingBoxId_Priority",
                schema: "dbo",
                table: "ShippingBoxRules",
                columns: new[] { "ShippingBoxId", "Priority" });

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
                name: "IX_ShippingDistances_MinDistanceKm_MaxDistanceKm",
                schema: "dbo",
                table: "ShippingDistances",
                columns: new[] { "MinDistanceKm", "MaxDistanceKm" });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDistances_Name",
                schema: "dbo",
                table: "ShippingDistances",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_StaffId",
                schema: "dbo",
                table: "StaffAssignments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_WorkScheduleId",
                schema: "dbo",
                table: "StaffAssignments",
                column: "WorkScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_IsDeleted",
                schema: "dbo",
                table: "TaskTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_TaskName",
                schema: "dbo",
                table: "TaskTemplates",
                column: "TaskName");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_ApplicationUserId",
                schema: "dbo",
                table: "UserDetails",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                schema: "dbo",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_VarietyName",
                schema: "dbo",
                table: "Varieties",
                column: "VarietyName",
                unique: true);

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
                name: "IX_VarietyPatterns_PatternId",
                schema: "dbo",
                table: "VarietyPatterns",
                column: "PatternId");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyPatterns_VarietyId",
                schema: "dbo",
                table: "VarietyPatterns",
                column: "VarietyId");

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
                name: "IX_WaterAlerts_WaterParameterRecordId",
                schema: "dbo",
                table: "WaterAlerts",
                column: "WaterParameterRecordId");

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
                name: "IX_WeeklyScheduleTemplateItems_TaskTemplateId",
                schema: "dbo",
                table: "WeeklyScheduleTemplateItems",
                column: "TaskTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyScheduleTemplateItems_WeeklyScheduleTemplateId",
                schema: "dbo",
                table: "WeeklyScheduleTemplateItems",
                column: "WeeklyScheduleTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_CreatedBy",
                schema: "dbo",
                table: "WorkSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ScheduledDate",
                schema: "dbo",
                table: "WorkSchedules",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ScheduledDate_Status",
                schema: "dbo",
                table: "WorkSchedules",
                columns: new[] { "ScheduledDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_Status",
                schema: "dbo",
                table: "WorkSchedules",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "TaskTemplateId");

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
                name: "AspNetRoleClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "dbo");

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
                name: "KoiFavorites",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "KoiGalleryEnrollment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "KoiIdentification",
                schema: "dbo");

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
                name: "PaymentTransactions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondAssignments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondIncidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondPacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshToken",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingBoxRules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingDistances",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "StaffAssignments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserDetails",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserLogin",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserToken",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VarietyPacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VarietyPatterns",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterAlerts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterParameterThresholds",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WeeklyScheduleTemplateItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Carts",
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
                name: "Orders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Incidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ShippingBoxes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WorkSchedules",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Patterns",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaterParameterRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WeeklyScheduleTemplates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CustomerAddresses",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Promotions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IncidentTypes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TaskTemplates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ApplicationUsers",
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
