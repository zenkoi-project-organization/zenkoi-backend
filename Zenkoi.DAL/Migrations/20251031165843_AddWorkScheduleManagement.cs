using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkScheduleManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskTemplates_ApplicationUsers_AssignedToUserId",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTemplates_Ponds_PondId",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_StaffId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_TaskTemplates_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_WorkDate",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_AssignedToUserId",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_PondId",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_ScheduledAt",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "ManagerNotes",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "PondId",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CheckedOutAt",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_WorkSchedules_StaffId",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "IX_WorkSchedules_CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "dbo",
                table: "TaskTemplates",
                newName: "TaskName");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                schema: "dbo",
                table: "TaskTemplates",
                newName: "DefaultDuration");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                schema: "dbo",
                table: "WorkSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true,
                oldDefaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                schema: "dbo",
                table: "WorkSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduledDate",
                schema: "dbo",
                table: "WorkSchedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "WorkSchedules",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                schema: "dbo",
                table: "TaskTemplates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "TaskTemplates",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "TaskTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "dbo",
                table: "TaskTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PondAssignments",
                schema: "dbo",
                columns: table => new
                {
                    WorkScheduleId = table.Column<int>(type: "int", nullable: false),
                    PondId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_TaskTemplates_IsDeleted",
                schema: "dbo",
                table: "TaskTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_IsRecurring",
                schema: "dbo",
                table: "TaskTemplates",
                column: "IsRecurring");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_TaskName",
                schema: "dbo",
                table: "TaskTemplates",
                column: "TaskName");

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_PondId",
                schema: "dbo",
                table: "PondAssignments",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondAssignments_WorkScheduleId",
                schema: "dbo",
                table: "PondAssignments",
                column: "WorkScheduleId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_CreatedBy",
                schema: "dbo",
                table: "WorkSchedules",
                column: "CreatedBy",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_TaskTemplates_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "TaskTemplateId",
                principalSchema: "dbo",
                principalTable: "TaskTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_CreatedBy",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_TaskTemplates_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropTable(
                name: "PondAssignments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "StaffAssignments",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ScheduledDate",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ScheduledDate_Status",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_Status",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_IsDeleted",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_IsRecurring",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TaskTemplates_TaskName",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "TaskTemplates");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "CheckedOutAt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkSchedules_CreatedBy",
                schema: "dbo",
                table: "WorkSchedules",
                newName: "IX_WorkSchedules_StaffId");

            migrationBuilder.RenameColumn(
                name: "TaskName",
                schema: "dbo",
                table: "TaskTemplates",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "DefaultDuration",
                schema: "dbo",
                table: "TaskTemplates",
                newName: "AssignedToUserId");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                schema: "dbo",
                table: "WorkSchedules",
                type: "time",
                nullable: true,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                schema: "dbo",
                table: "WorkSchedules",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                schema: "dbo",
                table: "WorkSchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ManagerNotes",
                schema: "dbo",
                table: "WorkSchedules",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkDate",
                schema: "dbo",
                table: "WorkSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                schema: "dbo",
                table: "TaskTemplates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PondId",
                schema: "dbo",
                table: "TaskTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                schema: "dbo",
                table: "TaskTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_WorkDate",
                schema: "dbo",
                table: "WorkSchedules",
                column: "WorkDate");

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

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTemplates_ApplicationUsers_AssignedToUserId",
                schema: "dbo",
                table: "TaskTemplates",
                column: "AssignedToUserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTemplates_Ponds_PondId",
                schema: "dbo",
                table: "TaskTemplates",
                column: "PondId",
                principalSchema: "dbo",
                principalTable: "Ponds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_CreatedByUserId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "CreatedByUserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_ApplicationUsers_StaffId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "StaffId",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_TaskTemplates_TaskTemplateId",
                schema: "dbo",
                table: "WorkSchedules",
                column: "TaskTemplateId",
                principalSchema: "dbo",
                principalTable: "TaskTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
