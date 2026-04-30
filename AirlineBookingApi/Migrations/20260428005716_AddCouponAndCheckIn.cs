using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponAndCheckIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxUses = table.Column<int>(type: "int", nullable: false),
                    CurrentUses = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "CurrentUses", "DiscountPercent", "ExpiryDate", "IsActive", "MaxUses", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "WELCOME10", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 10m, new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 100, null },
                    { 2, "SUMMER20", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 20m, new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 50, null },
                    { 3, "VIP30", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 30m, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 10, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
