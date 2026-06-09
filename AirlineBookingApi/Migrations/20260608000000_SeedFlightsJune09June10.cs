using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    public partial class SeedFlightsJune09June10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== 09/06/2026 (VN) =====
            // DateUTC = '2026-06-09' -> VN departure times: 06:00 ~ 20:00
            // HAN->SGN: 1h55
            // SGN->DAD: 1h20
            // DAD->HAN: 1h15
            // SGN->PQC: 0h55
            // SGN->SIN: 1h55

            migrationBuilder.Sql(@"
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(minute, r.DepM, CAST(r.DateUTC AS DATETIME2)),
       DATEADD(minute, r.ArrM, CAST(r.DateUTC AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- ===== 09/06/2026 (VN) =====
    ('VN16001', 'Ha Noi', 'Ho Chi Minh', '2026-06-09',  -60,   55, 'Scheduled', 'VN'),
    ('VJ16002', 'Ho Chi Minh', 'Da Nang',    '2026-06-09',  210,  290, 'Scheduled', 'VJ'),
    ('QH16003', 'Da Nang',     'Ha Noi',     '2026-06-09',  420,  495, 'Scheduled', 'QH'),
    ('BL16004', 'Ho Chi Minh', 'Phu Quoc',   '2026-06-09',  570,  625, 'Scheduled', 'BL'),
    ('SQ16005', 'Ho Chi Minh', 'Singapore',  '2026-06-09',  780,  895, 'Scheduled', 'SQ'),

    -- ===== 10/06/2026 (VN) =====
    ('VJ17001', 'Ho Chi Minh', 'Ha Noi',     '2026-06-10',  -60,   55, 'Scheduled', 'VJ'),
    ('BL17002', 'Ha Noi',      'Da Nang',    '2026-06-10',  180,  255, 'Scheduled', 'BL'),
    ('VN17003', 'Da Nang',     'Ho Chi Minh','2026-06-10',  390,  465, 'Scheduled', 'VN'),
    ('TG17004', 'Ha Noi',      'Bangkok',    '2026-06-10',  540,  660, 'Scheduled', 'TG'),
    ('QH17005', 'Ho Chi Minh', 'Phu Quoc',   '2026-06-10',  720,  780, 'Scheduled', 'QH')
) r(FlightNum, Dep, Arr, DateUTC, DepM, ArrM, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum);
");

            // Insert FlightPrices (Economy)
            migrationBuilder.Sql(@"
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN16001', 1800000), ('VJ16002', 1100000), ('QH16003', 1200000),
    ('BL16004',  950000), ('SQ16005', 3500000),
    ('VJ17001', 1500000), ('BL17002', 1000000), ('VN17003', 1300000),
    ('TG17004', 2800000), ('QH17005',  900000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');
");

            // Insert FlightPrices (Business)
            migrationBuilder.Sql(@"
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN16001', 3800000), ('VJ16002', 2600000), ('QH16003', 2800000),
    ('BL16004', 2200000), ('SQ16005', 7500000),
    ('VJ17001', 3400000), ('BL17002', 2400000), ('VN17003', 3000000),
    ('TG17004', 6000000), ('QH17005', 2100000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats (96 seats per flight: 24 Business + 72 Economy)
            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE f.FlightNumber IN (
        'VN16001','VJ16002','QH16003','BL16004','SQ16005',
        'VJ17001','BL17002','VN17003','TG17004','QH17005'
    )
    AND NOT EXISTS (SELECT 1 FROM Seats s WHERE s.FlightId = f.Id);

OPEN flightCur;
FETCH NEXT FROM flightCur INTO @flightId;
WHILE @@FETCH_STATUS = 0
BEGIN
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM FlightPrices WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE FlightNumber IN (
        'VN16001','VJ16002','QH16003','BL16004','SQ16005',
        'VJ17001','BL17002','VN17003','TG17004','QH17005'
    )
);
DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE FlightNumber IN (
        'VN16001','VJ16002','QH16003','BL16004','SQ16005',
        'VJ17001','BL17002','VN17003','TG17004','QH17005'
    )
);
DELETE FROM Flights WHERE FlightNumber IN (
    'VN16001','VJ16002','QH16003','BL16004','SQ16005',
    'VJ17001','BL17002','VN17003','TG17004','QH17005'
);
");
        }
    }
}
