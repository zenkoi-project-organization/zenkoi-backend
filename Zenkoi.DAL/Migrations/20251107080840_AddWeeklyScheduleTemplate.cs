using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyScheduleTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "WeeklyScheduleTemplateItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeeklyScheduleTemplateId = table.Column<int>(type: "int", nullable: false),
                    TaskTemplateId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeeklyScheduleTemplateItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WeeklyScheduleTemplates",
                schema: "dbo");
        }
    }
}
