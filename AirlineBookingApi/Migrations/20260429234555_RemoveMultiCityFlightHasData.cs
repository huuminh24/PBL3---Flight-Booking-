using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMultiCityFlightHasData : Migration
    {
        // NOTE: This migration only removes the HasData declarations for VN2002/VN3002
        // (Flights 5,6 / FlightPrices 9-12 / Seats 33-48) from the model snapshot.
        // The actual rows are now seeded via raw SQL (looked up by FlightNumber) inside
        // the AddMultiCityFlightsAndPaymentCoupon migration with auto-generated Ids,
        // so we intentionally do NOT execute any DeleteData / InsertData here.
        // Touching them would either delete real seeded rows (cascading Seats/FlightPrices)
        // on a fresh DB, or hit Id mismatches on legacy DBs.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
