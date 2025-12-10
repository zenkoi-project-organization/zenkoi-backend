using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MoveImagesFromWorkScheduleToStaffAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                schema: "dbo",
                table: "StaffAssignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                schema: "dbo",
                table: "StaffAssignments");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                schema: "dbo",
                table: "WorkSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
