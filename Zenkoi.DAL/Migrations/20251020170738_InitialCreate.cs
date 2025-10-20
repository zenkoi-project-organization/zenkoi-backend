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
                    Images = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Video = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacketFishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PondTypes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecommendedCapacity = table.Column<int>(type: "int", nullable: true)
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
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
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
                name: "PaymentTransactions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ResponseData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                    DepthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    LengthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    WidthMeters = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
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
                name: "BreedingProcesses",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaleKoiId = table.Column<int>(type: "int", nullable: false),
                    FemaleKoiId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Result = table.Column<int>(type: "int", nullable: false),
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
                    PondId = table.Column<int>(type: "int", nullable: false),
                    TotalCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    QualifiedCount = table.Column<int>(type: "int", nullable: true),
                    UnqualifiedCount = table.Column<int>(type: "int", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_ClassificationStages_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
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
                    PondId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    TotalHatchedEggs = table.Column<int>(type: "int", nullable: true),
                    FertilizationRate = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    PondId = table.Column<int>(type: "int", nullable: true),
                    InitialCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentSurvivalRate = table.Column<double>(type: "float", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Size = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagesVideos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BodyShape = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
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
                name: "ClassificationRecords",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassificationStageId = table.Column<int>(type: "int", nullable: false),
                    StageNumber = table.Column<int>(type: "int", nullable: false),
                    HighQualifiedCount = table.Column<int>(type: "int", nullable: true),
                    QualifiedCount = table.Column<int>(type: "int", nullable: true),
                    UnqualifiedCount = table.Column<int>(type: "int", nullable: true),
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
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    HealthyEggs = table.Column<int>(type: "int", nullable: true),
                    RottenEggs = table.Column<int>(type: "int", nullable: true),
                    HatchedEggs = table.Column<int>(type: "int", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_ClassificationStages_PondId",
                schema: "dbo",
                table: "ClassificationStages",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApplicationUserId",
                schema: "dbo",
                table: "Customers",
                column: "ApplicationUserId",
                unique: true);

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
                name: "ClassificationRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FrySurvivalRecords",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IncubationDailyRecords",
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
                name: "PondIncidents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PondPacketFishes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshToken",
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
                name: "Roles",
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
