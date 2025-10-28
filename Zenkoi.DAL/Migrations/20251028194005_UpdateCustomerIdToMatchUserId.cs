using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerIdToMatchUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ApplicationUsers_ApplicationUserId",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ApplicationUserId",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "dbo",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ApplicationUsers_Id",
                schema: "dbo",
                table: "Customers",
                column: "Id",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ApplicationUsers_Id",
                schema: "dbo",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "dbo",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                schema: "dbo",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ApplicationUserId",
                schema: "dbo",
                table: "Customers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ApplicationUsers_ApplicationUserId",
                schema: "dbo",
                table: "Customers",
                column: "ApplicationUserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
