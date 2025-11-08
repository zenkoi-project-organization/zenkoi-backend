using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKoiFishEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyShape",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "ColorPattern",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsMutated",
                schema: "dbo",
                table: "KoiFishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MutationRate",
                schema: "dbo",
                table: "KoiFishes",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MutationType",
                schema: "dbo",
                table: "KoiFishes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMutated",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "MutationRate",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.DropColumn(
                name: "MutationType",
                schema: "dbo",
                table: "KoiFishes");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)");

            migrationBuilder.AddColumn<string>(
                name: "BodyShape",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorPattern",
                schema: "dbo",
                table: "KoiFishes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
