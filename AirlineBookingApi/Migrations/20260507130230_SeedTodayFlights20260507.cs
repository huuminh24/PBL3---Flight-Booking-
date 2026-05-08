using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedTodayFlights20260507 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Seed flights for 2026-05-07 (today) with UTC times corresponding to Vietnam local hours 07:00-21:00
DECLARE @TodayUtc DATETIME = '2026-05-07T00:00:00';
DECLARE @AirlineVN INT = (SELECT TOP 1 Id FROM Airlines WHERE Code = 'VN');
DECLARE @AirlineVJ INT = (SELECT TOP 1 Id FROM Airlines WHERE Code = 'VJ');

IF @AirlineVN IS NULL OR @AirlineVJ IS NULL RETURN;

-- Flights table: FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime(UTC), ArrivalTime(UTC)
-- 07:00 VN = 00:00 UTC, 10:00 VN = 03:00 UTC, 14:00 VN = 07:00 UTC, 18:00 VN = 11:00 UTC

INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt, AirlineId)
SELECT FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, N'Scheduled', GETUTCDATE(), NULL, AirlineId
FROM (VALUES
    (N'TODAY01', N'Da Nang',     N'Ho Chi Minh', DATEADD(HOUR, 0,  @TodayUtc), DATEADD(HOUR, 1,  @TodayUtc), @AirlineVN), -- 07:00 VN
    (N'TODAY02', N'Ho Chi Minh', N'Da Nang',     DATEADD(HOUR, 3,  @TodayUtc), DATEADD(HOUR, 4,  @TodayUtc), @AirlineVJ), -- 10:00 VN
    (N'TODAY03', N'Ha Noi',      N'Da Nang',     DATEADD(HOUR, 7,  @TodayUtc), DATEADD(HOUR, 8,  @TodayUtc), @AirlineVN), -- 14:00 VN
    (N'TODAY04', N'Da Nang',     N'Ha Noi',      DATEADD(HOUR, 11, @TodayUtc), DATEADD(HOUR, 12, @TodayUtc), @AirlineVJ), -- 18:00 VN
    (N'TODAY05', N'Ha Noi',      N'Ho Chi Minh', DATEADD(HOUR, 0,  @TodayUtc), DATEADD(HOUR, 2,  @TodayUtc), @AirlineVN), -- 07:00 VN
    (N'TODAY06', N'Ho Chi Minh', N'Ha Noi',      DATEADD(HOUR, 3,  @TodayUtc), DATEADD(HOUR, 5,  @TodayUtc), @AirlineVJ)  -- 10:00 VN
) AS v(FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, AirlineId)
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = v.FlightNumber AND CONVERT(date, f.DepartureTime) = CAST(@TodayUtc AS DATE));

-- Insert FlightPrices
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', 1200000, GETUTCDATE(), NULL
FROM Flights f
WHERE f.FlightNumber LIKE N'TODAY%' AND CONVERT(date, f.DepartureTime) = @TodayUtc
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', 2800000, GETUTCDATE(), NULL
FROM Flights f
WHERE f.FlightNumber LIKE N'TODAY%' AND CONVERT(date, f.DepartureTime) = @TodayUtc
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');

-- Insert Economy Seats: 9 rows × 6 cols = 54 per flight
WITH EconRows AS (
    SELECT 1 AS r UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
    UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9
)
INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
SELECT f.Id, 'E' + CAST(r.r AS NVARCHAR(2)) + CHAR(64 + s.c), 'Economy', 1, GETUTCDATE(), NULL
FROM Flights f
CROSS JOIN EconRows r
CROSS JOIN (SELECT 1 AS c UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6) s
WHERE f.FlightNumber LIKE N'TODAY%' AND CONVERT(date, f.DepartureTime) = CAST(@TodayUtc AS DATE)
AND NOT EXISTS (SELECT 1 FROM Seats st WHERE st.FlightId = f.Id AND st.SeatNumber = 'E' + CAST(r.r AS NVARCHAR(2)) + CHAR(64 + s.c));

-- Insert Business Seats: 7 rows × 2 cols = 14 per flight
WITH BizRows AS (
    SELECT 1 AS r UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
    UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7
)
INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
SELECT f.Id, 'B' + CAST(r.r AS NVARCHAR(2)) + CHAR(64 + s.c), 'Business', 1, GETUTCDATE(), NULL
FROM Flights f
CROSS JOIN BizRows r
CROSS JOIN (SELECT 1 AS c UNION ALL SELECT 2) s
WHERE f.FlightNumber LIKE N'TODAY%' AND CONVERT(date, f.DepartureTime) = CAST(@TodayUtc AS DATE)
AND NOT EXISTS (SELECT 1 FROM Seats st WHERE st.FlightId = f.Id AND st.SeatNumber = 'B' + CAST(r.r AS NVARCHAR(2)) + CHAR(64 + s.c));
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
