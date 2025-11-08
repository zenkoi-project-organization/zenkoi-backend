using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsRecurringWithNotesTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_IsRecurring",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "RecurrenceRule",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.AddColumn<string>(
                name: "NotesTask",
                schema: "dbo",
                table: "TaskTemplates",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotesTask",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                schema: "dbo",
                table: "TaskTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceRule",
                schema: "dbo",
                table: "TaskTemplates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_IsRecurring",
                schema: "dbo",
                table: "TaskTemplates",
                column: "IsRecurring");
        }
    }
}
