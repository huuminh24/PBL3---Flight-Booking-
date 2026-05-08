using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedFlightUtcTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // These seed flights were stored with VN local hours directly as UTC (missing -7h offset).
            // A flight at "18:00 UTC" = 01:00 AM VN next day → never shows when searching the intended VN date.
            // Fix: subtract 7h so the UTC value correctly represents the intended VN local departure time.
            migrationBuilder.Sql(@"
-- VN5004: Da Nang → Ho Chi Minh, intended 18:00 VN → should be 11:00 UTC
UPDATE Flights SET
    DepartureTime = '2026-05-08T11:00:00',
    ArrivalTime   = '2026-05-08T12:30:00'
WHERE FlightNumber = 'VN5004'
  AND CONVERT(date, DepartureTime) = '2026-05-08'
  AND DATEPART(hour, DepartureTime) = 18;

-- QH302: Ha Noi → Da Nang, intended 17:00 VN → should be 10:00 UTC
UPDATE Flights SET
    DepartureTime = '2026-05-14T10:00:00',
    ArrivalTime   = '2026-05-14T12:00:00'
WHERE FlightNumber = 'QH302'
  AND CONVERT(date, DepartureTime) = '2026-05-14'
  AND DATEPART(hour, DepartureTime) = 17;

-- VJ402: Da Nang → Ha Noi, intended 18:00 VN → should be 11:00 UTC
UPDATE Flights SET
    DepartureTime = '2026-05-14T11:00:00',
    ArrivalTime   = '2026-05-14T12:00:00'
WHERE FlightNumber = 'VJ402'
  AND CONVERT(date, DepartureTime) = '2026-05-14'
  AND DATEPART(hour, DepartureTime) = 18;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
