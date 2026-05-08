using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlightsMay0809102026V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed 6 flights for dates 09-12 May 2026 (Vietnam timezone)
            // 09/05/2026 VN = 2026-05-08 UTC
            // 10/05/2026 VN = 2026-05-09 UTC
            // 11/05/2026 VN = 2026-05-10 UTC
            // 12/05/2026 VN = 2026-05-11 UTC

            migrationBuilder.Sql(@"
-- Insert Flights for 09-12 May 2026
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(r.DateUTC AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(r.DateUTC AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- 09/05/2026 VN = 2026-05-08 UTC
    ('VN8001', 'Ha Noi', 'Ho Chi Minh', '2026-05-08', -1, 1, 'Scheduled', 'VN'),
    ('VJ8002', 'Ho Chi Minh', 'Ha Noi', '2026-05-08', 2, 4, 'Scheduled', 'VJ'),
    -- 10/05/2026 VN = 2026-05-09 UTC
    ('VN9001', 'Da Nang', 'Ho Chi Minh', '2026-05-09', 3, 4, 'Scheduled', 'VN'),
    ('QH9002', 'Ho Chi Minh', 'Da Nang', '2026-05-09', 5, 6, 'Scheduled', 'QH'),
    -- 11/05/2026 VN = 2026-05-10 UTC
    ('VN10001', 'Ha Noi', 'Phu Quoc', '2026-05-10', 0, 2, 'Scheduled', 'VN'),
    -- 12/05/2026 VN = 2026-05-11 UTC
    ('SQ11001', 'Ho Chi Minh', 'Singapore', '2026-05-11', 4, 6, 'Scheduled', 'SQ')
) r(FlightNum, Dep, Arr, DateUTC, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum);
");

            // Insert FlightPrices for the new flights
            migrationBuilder.Sql(@"
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN8001', 1500000),
    ('VJ8002', 1600000),
    ('VN9001', 900000),
    ('QH9002', 1000000),
    ('VN10001', 1200000),
    ('SQ11001', 3500000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN8001', 3500000),
    ('VJ8002', 3700000),
    ('VN9001', 2200000),
    ('QH9002', 2400000),
    ('VN10001', 2800000),
    ('SQ11001', 8000000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats for the new flights
            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE f.FlightNumber IN ('VN8001', 'VJ8002', 'VN9001', 'QH9002', 'VN10001', 'SQ11001')
    AND NOT EXISTS (SELECT 1 FROM Seats s WHERE s.FlightId = f.Id);

OPEN flightCur;
FETCH NEXT FROM flightCur INTO @flightId;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Business: rows 1-6, columns A-D (4 per row = 24 seats)
    SET @row = 1;
    WHILE @row <= 6
    BEGIN
        SET @col = 1;
        WHILE @col <= 4
        BEGIN
            SET @sn = N'B' + CAST(@row AS NVARCHAR(2)) + CHAR(64 + @col);
            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
            VALUES (@flightId, @sn, N'Business', 1, GETUTCDATE(), GETUTCDATE());
            SET @col += 1;
        END
        SET @row += 1;
    END

    -- Economy: rows 1-12, columns A-F (6 per row = 72 seats)
    SET @row = 1;
    WHILE @row <= 12
    BEGIN
        SET @col = 1;
        WHILE @col <= 6
        BEGIN
            SET @sn = N'E' + CAST(@row AS NVARCHAR(2)) + CHAR(64 + @col);
            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
            VALUES (@flightId, @sn, N'Economy', 1, GETUTCDATE(), GETUTCDATE());
            SET @col += 1;
        END
        SET @row += 1;
    END

    FETCH NEXT FROM flightCur INTO @flightId;
END
CLOSE flightCur;
DEALLOCATE flightCur;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the seeded flights and related data
            migrationBuilder.Sql(@"
DELETE FROM FlightPrices WHERE FlightId IN (
    SELECT Id FROM Flights WHERE FlightNumber IN ('VN8001', 'VJ8002', 'VN9001', 'QH9002', 'VN10001', 'SQ11001')
);

DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights WHERE FlightNumber IN ('VN8001', 'VJ8002', 'VN9001', 'QH9002', 'VN10001', 'SQ11001')
);

DELETE FROM Flights WHERE FlightNumber IN ('VN8001', 'VJ8002', 'VN9001', 'QH9002', 'VN10001', 'SQ11001');
");
        }
    }
}
