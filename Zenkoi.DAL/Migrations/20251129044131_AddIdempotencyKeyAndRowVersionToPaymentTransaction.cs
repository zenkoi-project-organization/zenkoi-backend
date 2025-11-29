using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenkoi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyAndRowVersionToPaymentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "dbo",
                table: "PaymentTransactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "dbo",
                table: "PaymentTransactions",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_IdempotencyKey",
                schema: "dbo",
                table: "PaymentTransactions",
                column: "IdempotencyKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_IdempotencyKey",
                schema: "dbo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "dbo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "dbo",
                table: "PaymentTransactions");
        }
    }
}
