using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedMultiCityTestFlights : Migration
    {
        // Seeds 6 connecting flights (covering DAD-SGN-HAN, HAN-DAD-SGN, SGN-HAN-DAD)
        // for end-to-end multi-city testing between 2026-05-01 and 2026-05-15.
        // All inserts are idempotent (matched by FlightNumber and (FlightId, SeatNumber)).

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Each row: FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime (UTC), ArrivalTime (UTC).
            migrationBuilder.Sql(@"
;WITH SeedFlights(FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime) AS (
    SELECT N'VN5001', N'Da Nang',     N'Ho Chi Minh', '2026-05-03T07:00:00', '2026-05-03T08:30:00' UNION ALL
    SELECT N'VN5002', N'Ho Chi Minh', N'Ha Noi',      '2026-05-03T11:00:00', '2026-05-03T13:10:00' UNION ALL
    SELECT N'VN5003', N'Ha Noi',      N'Da Nang',     '2026-05-08T15:00:00', '2026-05-08T16:25:00' UNION ALL
    SELECT N'VN5004', N'Da Nang',     N'Ho Chi Minh', '2026-05-08T18:00:00', '2026-05-08T19:30:00' UNION ALL
    SELECT N'VN5005', N'Ho Chi Minh', N'Ha Noi',      '2026-05-12T06:30:00', '2026-05-12T08:40:00' UNION ALL
    SELECT N'VN5006', N'Ha Noi',      N'Da Nang',     '2026-05-15T09:00:00', '2026-05-15T10:25:00'
)
INSERT INTO [Flights] ([ArrivalAirport], [ArrivalTime], [CreatedAt], [DepartureAirport], [DepartureTime], [FlightNumber], [Status], [UpdatedAt])
SELECT s.ArrivalAirport, s.ArrivalTime, '2026-01-01T00:00:00', s.DepartureAirport, s.DepartureTime, s.FlightNumber, N'Scheduled', NULL
FROM SeedFlights s
WHERE NOT EXISTS (SELECT 1 FROM [Flights] f WHERE f.[FlightNumber] = s.FlightNumber);
");

            migrationBuilder.Sql(@"DBCC CHECKIDENT('[Flights]', RESEED);");

            // Prices for the seeded flights (Economy + Business).
            migrationBuilder.Sql(@"
;WITH SeedPrices(FlightNumber, SeatClass, Price) AS (
    SELECT N'VN5001', N'Economy',  1300000 UNION ALL SELECT N'VN5001', N'Business', 2400000 UNION ALL
    SELECT N'VN5002', N'Economy',  1500000 UNION ALL SELECT N'VN5002', N'Business', 2700000 UNION ALL
    SELECT N'VN5003', N'Economy',  1450000 UNION ALL SELECT N'VN5003', N'Business', 2550000 UNION ALL
    SELECT N'VN5004', N'Economy',  1350000 UNION ALL SELECT N'VN5004', N'Business', 2450000 UNION ALL
    SELECT N'VN5005', N'Economy',  1550000 UNION ALL SELECT N'VN5005', N'Business', 2750000 UNION ALL
    SELECT N'VN5006', N'Economy',  1400000 UNION ALL SELECT N'VN5006', N'Business', 2500000
)
INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], sp.Price, sp.SeatClass, NULL
FROM SeedPrices sp
INNER JOIN [Flights] f ON f.[FlightNumber] = sp.FlightNumber
WHERE NOT EXISTS (
    SELECT 1 FROM [FlightPrices] fp
    WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = sp.SeatClass
);
");

            // Seats: 5 Economy (E01-E05) + 3 Business (B01-B03) for each VN5xxx flight.
            migrationBuilder.Sql(@"
;WITH SeatCatalog(SeatNumber, SeatClass) AS (
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
SELECT '2026-01-01T00:00:00', f.[Id], 1, sc.SeatClass, sc.SeatNumber, NULL
FROM [Flights] f
CROSS JOIN SeatCatalog sc
WHERE f.[FlightNumber] IN (N'VN5001', N'VN5002', N'VN5003', N'VN5004', N'VN5005', N'VN5006')
  AND NOT EXISTS (
      SELECT 1 FROM [Seats] s
      WHERE s.[FlightId] = f.[Id] AND s.[SeatNumber] = sc.SeatNumber
  );
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @FlightNumbers TABLE ([FlightNumber] NVARCHAR(20));
INSERT INTO @FlightNumbers VALUES (N'VN5001'), (N'VN5002'), (N'VN5003'), (N'VN5004'), (N'VN5005'), (N'VN5006');

DELETE FROM [Seats]
WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] IN (SELECT [FlightNumber] FROM @FlightNumbers));

DELETE FROM [FlightPrices]
WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] IN (SELECT [FlightNumber] FROM @FlightNumbers));

DELETE FROM [Flights] WHERE [FlightNumber] IN (SELECT [FlightNumber] FROM @FlightNumbers);
");
        }
    }
}
