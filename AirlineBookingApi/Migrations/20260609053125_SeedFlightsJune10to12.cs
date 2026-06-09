using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlightsJune10to12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(minute, r.DepM, CAST(r.DateUTC AS DATETIME2)),
       DATEADD(minute, r.ArrM, CAST(r.DateUTC AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    ('VN18001', 'Ha Noi',      'Ho Chi Minh', '2026-06-10',  -60,   55, 'Scheduled', 'VN'),
    ('VJ18002', 'Ho Chi Minh', 'Da Nang',     '2026-06-10',  150,  230, 'Scheduled', 'VJ'),
    ('QH18003', 'Da Nang',     'Ha Noi',      '2026-06-10',  300,  375, 'Scheduled', 'QH'),
    ('BL18004', 'Ha Noi',      'Phu Quoc',    '2026-06-10',  420,  535, 'Scheduled', 'BL'),
    ('SQ18005', 'Ho Chi Minh', 'Singapore',   '2026-06-10',  540,  660, 'Scheduled', 'SQ'),
    ('TG18006', 'Ha Noi',      'Bangkok',     '2026-06-10',  660,  780, 'Scheduled', 'TG'),
    ('VJ18007', 'Ho Chi Minh', 'Ha Noi',      '2026-06-10',  780,  895, 'Scheduled', 'VJ'),
    ('VN18008', 'Ho Chi Minh', 'Ha Noi',      '2026-06-11',  -60,   55, 'Scheduled', 'VN'),
    ('VJ18009', 'Ha Noi',      'Da Nang',     '2026-06-11',  120,  195, 'Scheduled', 'VJ'),
    ('QH18010', 'Da Nang',     'Ho Chi Minh', '2026-06-11',  300,  380, 'Scheduled', 'QH'),
    ('BL18011', 'Ho Chi Minh', 'Nha Trang',   '2026-06-11',  420,  490, 'Scheduled', 'BL'),
    ('SQ18012', 'Ha Noi',      'Singapore',   '2026-06-11',  540,  660, 'Scheduled', 'SQ'),
    ('TG18013', 'Ho Chi Minh', 'Bangkok',     '2026-06-11',  660,  780, 'Scheduled', 'TG'),
    ('VN18014', 'Ha Noi',      'Phu Quoc',    '2026-06-11',  780,  895, 'Scheduled', 'VN'),
    ('VN18015', 'Ho Chi Minh', 'Ha Noi',      '2026-06-12',  -60,   55, 'Scheduled', 'VN'),
    ('VJ18016', 'Da Nang',     'Ho Chi Minh', '2026-06-12',  180,  260, 'Scheduled', 'VJ'),
    ('QH18017', 'Ha Noi',      'Da Nang',     '2026-06-12',  360,  435, 'Scheduled', 'QH'),
    ('BL18018', 'Ho Chi Minh', 'Phu Quoc',    '2026-06-12',  480,  535, 'Scheduled', 'BL'),
    ('SQ18019', 'Ho Chi Minh', 'Singapore',   '2026-06-12',  600,  720, 'Scheduled', 'SQ'),
    ('TG18020', 'Ha Noi',      'Bangkok',     '2026-06-12',  720,  840, 'Scheduled', 'TG')
) r(FlightNum, Dep, Arr, DateUTC, DepM, ArrM, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum);
");

            migrationBuilder.Sql(@"
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN18001', 1850000), ('VJ18002', 1150000), ('QH18003', 1250000),
    ('BL18004', 1450000), ('SQ18005', 3600000), ('TG18006', 2750000),
    ('VJ18007', 1550000), ('VN18008', 1650000), ('VJ18009', 1050000),
    ('QH18010', 1350000), ('BL18011',  900000), ('SQ18012', 3300000),
    ('TG18013', 2550000), ('VN18014', 1400000), ('VN18015', 1750000),
    ('VJ18016', 1100000), ('QH18017', 1200000), ('BL18018',  950000),
    ('SQ18019', 3450000), ('TG18020', 2650000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');
");

            migrationBuilder.Sql(@"
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN18001', 4300000), ('VJ18002', 2700000), ('QH18003', 2950000),
    ('BL18004', 3400000), ('SQ18005', 7800000), ('TG18006', 6100000),
    ('VJ18007', 3600000), ('VN18008', 3850000), ('VJ18009', 2500000),
    ('QH18010', 3150000), ('BL18011', 2100000), ('SQ18012', 7200000),
    ('TG18013', 5700000), ('VN18014', 3250000), ('VN18015', 4100000),
    ('VJ18016', 2600000), ('QH18017', 2850000), ('BL18018', 2200000),
    ('SQ18019', 7500000), ('TG18020', 5900000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR
    SELECT f.Id FROM Flights f
    WHERE f.FlightNumber IN (
        'VN18001','VJ18002','QH18003','BL18004','SQ18005','TG18006','VJ18007',
        'VN18008','VJ18009','QH18010','BL18011','SQ18012','TG18013','VN18014',
        'VN18015','VJ18016','QH18017','BL18018','SQ18019','TG18020'
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM FlightPrices WHERE FlightId IN (
    SELECT Id FROM Flights WHERE FlightNumber LIKE 'VN18%' OR FlightNumber LIKE 'VJ18%'
        OR FlightNumber LIKE 'QH18%' OR FlightNumber LIKE 'BL18%'
        OR FlightNumber LIKE 'SQ18%' OR FlightNumber LIKE 'TG18%'
);
DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights WHERE FlightNumber LIKE 'VN18%' OR FlightNumber LIKE 'VJ18%'
        OR FlightNumber LIKE 'QH18%' OR FlightNumber LIKE 'BL18%'
        OR FlightNumber LIKE 'SQ18%' OR FlightNumber LIKE 'TG18%'
);
DELETE FROM Flights WHERE FlightNumber LIKE 'VN18%' OR FlightNumber LIKE 'VJ18%'
    OR FlightNumber LIKE 'QH18%' OR FlightNumber LIKE 'BL18%'
    OR FlightNumber LIKE 'SQ18%' OR FlightNumber LIKE 'TG18%';
");
        }
    }
}
