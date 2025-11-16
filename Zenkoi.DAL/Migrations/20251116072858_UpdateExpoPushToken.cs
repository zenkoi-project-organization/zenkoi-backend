using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpoPushToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpoPushToken",
                schema: "dbo",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpoPushToken",
                schema: "dbo",
                table: "ApplicationUsers");
        }
    }
}
