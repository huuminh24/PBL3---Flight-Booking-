using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiCityFlightsAndPaymentCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Idempotent seed for multi-city flights. Match by FlightNumber so we don't conflict
            // with rows already created on legacy DBs that occupied Id=5,6 (e.g., SeedTestFlights).
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Flights] WHERE [FlightNumber] = N'VN2002')
BEGIN
    INSERT INTO [Flights] ([ArrivalAirport], [ArrivalTime], [CreatedAt], [DepartureAirport], [DepartureTime], [FlightNumber], [Status], [UpdatedAt])
    VALUES (N'Ha Noi', '2026-05-01T16:00:00', '2026-01-01T00:00:00', N'Ho Chi Minh', '2026-05-01T14:00:00', N'VN2002', N'Scheduled', NULL);
END
IF NOT EXISTS (SELECT 1 FROM [Flights] WHERE [FlightNumber] = N'VN3002')
BEGIN
    INSERT INTO [Flights] ([ArrivalAirport], [ArrivalTime], [CreatedAt], [DepartureAirport], [DepartureTime], [FlightNumber], [Status], [UpdatedAt])
    VALUES (N'Ho Chi Minh', '2026-05-02T11:00:00', '2026-01-01T00:00:00', N'Ha Noi', '2026-05-02T09:00:00', N'VN3002', N'Scheduled', NULL);
END
");

            // Reseed IDENTITY so subsequent inserts get correct next Id.
            migrationBuilder.Sql(@"DBCC CHECKIDENT('[Flights]', RESEED);");

            // FlightPrices: lookup the actual Flight Id via FlightNumber instead of hard-coding Id.
            migrationBuilder.Sql(@"
INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 1600000, N'Economy', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN2002'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Economy');

INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 2800000, N'Business', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN2002'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Business');

INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 1600000, N'Economy', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN3002'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Economy');

INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 2800000, N'Business', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN3002'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Business');
");

            // Seats: lookup actual Flight Id via FlightNumber, only insert missing (FlightId, SeatNumber) pairs.
            migrationBuilder.Sql(@"
;WITH NewSeats(SeatNumber, SeatClass) AS (
    SELECT N'E01', N'Economy' UNION ALL
    SELECT N'E02', N'Economy' UNION ALL
    SELECT N'E03', N'Economy' UNION ALL
    SELECT N'E04', N'Economy' UNION ALL
    SELECT N'E05', N'Economy' UNION ALL
    SELECT N'B01', N'Business' UNION ALL
    SELECT N'B02', N'Business' UNION ALL
    SELECT N'B03', N'Business'
)
INSERT INTO [Seats] ([CreatedAt], [FlightId], [IsAvailable], [SeatClass], [SeatNumber], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 1, n.SeatClass, n.SeatNumber, NULL
FROM [Flights] f
CROSS JOIN NewSeats n
WHERE f.[FlightNumber] IN (N'VN2002', N'VN3002')
  AND NOT EXISTS (
      SELECT 1 FROM [Seats] s
      WHERE s.[FlightId] = f.[Id] AND s.[SeatNumber] = n.SeatNumber
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Match the FlightNumber-based Up: delete by FlightId resolved through FlightNumber.
            migrationBuilder.Sql(@"
DELETE FROM [Seats]
WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] IN (N'VN2002', N'VN3002'));

DELETE FROM [FlightPrices]
WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] IN (N'VN2002', N'VN3002'));

DELETE FROM [Flights] WHERE [FlightNumber] IN (N'VN2002', N'VN3002');
");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Payments");
        }
    }
}
