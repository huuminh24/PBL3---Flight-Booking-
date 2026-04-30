using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusIndexAndPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings",
                column: "BookingStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingStatus",
                table: "Bookings");
        }
    }
}
