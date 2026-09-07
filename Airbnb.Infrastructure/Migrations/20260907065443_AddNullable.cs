using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airbnb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ExternalId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_ExternalId",
                table: "Apartments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ExternalId",
                table: "Bookings",
                column: "ExternalId",
                unique: true,
                filter: "[ExternalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ExternalId",
                table: "Apartments",
                column: "ExternalId",
                unique: true,
                filter: "[ExternalId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_ExternalId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_ExternalId",
                table: "Apartments");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ExternalId",
                table: "Bookings",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_ExternalId",
                table: "Apartments",
                column: "ExternalId",
                unique: true);
        }
    }
}
