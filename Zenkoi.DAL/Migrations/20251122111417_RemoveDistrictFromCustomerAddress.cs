using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDistrictFromCustomerAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                schema: "dbo",
                table: "CustomerAddresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                schema: "dbo",
                table: "CustomerAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
