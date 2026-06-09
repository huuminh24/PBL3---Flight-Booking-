using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokenToAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Accounts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiresAt",
                table: "Accounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordResetToken", "PasswordResetTokenExpiresAt" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordResetToken", "PasswordResetTokenExpiresAt" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiresAt",
                table: "Accounts");
        }
    }
}
