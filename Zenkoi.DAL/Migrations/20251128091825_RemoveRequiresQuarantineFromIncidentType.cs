using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequiresQuarantineFromIncidentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresQuarantine",
                schema: "dbo",
                table: "IncidentTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresQuarantine",
                schema: "dbo",
                table: "IncidentTypes",
                type: "bit",
                nullable: true,
                defaultValue: false);
        }
    }
}
