using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAirlinesAndAirlineId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AirlineId",
                table: "Flights",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LogoColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Airlines",
                columns: new[] { "Id", "Code", "CreatedAt", "LogoColor", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "VN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FFCC00", "Vietnam Airlines", null },
                    { 2, "VJ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#E91E63", "VietJet Air", null },
                    { 3, "QH", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#4CAF50", "Bamboo Airways", null },
                    { 4, "BL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FF5722", "Jetstar Pacific", null },
                    { 5, "SQ", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#F5A623", "Singapore Airlines", null },
                    { 6, "TG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#9C27B0", "Thai Airways", null }
                });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 1,
                column: "AirlineId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 2,
                column: "AirlineId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 3,
                column: "AirlineId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 4,
                column: "AirlineId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AirlineId",
                table: "Flights",
                column: "AirlineId");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_Code",
                table: "Airlines",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airlines_AirlineId",
                table: "Flights",
                column: "AirlineId",
                principalTable: "Airlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airlines_AirlineId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropIndex(
                name: "IX_Flights_AirlineId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "AirlineId",
                table: "Flights");
        }
    }
}
