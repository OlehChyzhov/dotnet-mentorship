using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StripeSessionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StripePaymentIntendId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StripePaymentSessionUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RefundStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PaymentSessionStatus = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_BookingId",
                table: "PaymentTransactions",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");
        }
    }
}
