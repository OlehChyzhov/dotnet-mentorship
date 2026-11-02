using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BookingSagaStateMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RandomValue",
                table: "SagaInstances");

            migrationBuilder.RenameColumn(
                name: "SomeId",
                table: "SagaInstances",
                newName: "BookingId");

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "SagaInstances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntendId",
                table: "SagaInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "SagaInstances");

            migrationBuilder.DropColumn(
                name: "PaymentIntendId",
                table: "SagaInstances");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "SagaInstances",
                newName: "SomeId");

            migrationBuilder.AddColumn<string>(
                name: "RandomValue",
                table: "SagaInstances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
