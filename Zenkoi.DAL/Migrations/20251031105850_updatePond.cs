using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatePond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentCount",
                schema: "dbo",
                table: "Ponds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxFishCount",
                schema: "dbo",
                table: "Ponds",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCount",
                schema: "dbo",
                table: "Ponds");

            migrationBuilder.DropColumn(
                name: "MaxFishCount",
                schema: "dbo",
                table: "Ponds");
        }
    }
}
