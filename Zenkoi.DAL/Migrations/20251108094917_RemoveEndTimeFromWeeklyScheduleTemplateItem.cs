using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEndTimeFromWeeklyScheduleTemplateItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "dbo",
                table: "WeeklyScheduleTemplateItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                schema: "dbo",
                table: "WeeklyScheduleTemplateItems",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
