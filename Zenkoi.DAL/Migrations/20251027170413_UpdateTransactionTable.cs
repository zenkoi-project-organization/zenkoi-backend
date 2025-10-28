using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "ActualOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Orders_ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "ActualOrderId",
                principalSchema: "dbo",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Orders_ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ActualOrderId",
                schema: "dbo",
                table: "PaymentTransactions");
        }
    }
}
