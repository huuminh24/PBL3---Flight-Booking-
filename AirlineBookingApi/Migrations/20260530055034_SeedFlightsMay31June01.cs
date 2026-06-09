using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    public partial class SeedFlightsMay31June01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 31/05/2026 VN = 2026-05-30 UTC
            // 01/06/2026 VN = 2026-05-31 UTC
            // Thoi gian bay duoc lam tu nhien: khong chuyen nao giong nhau

            migrationBuilder.Sql(@"
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(minute, r.DepM, CAST(r.DateUTC AS DATETIME2)),
       DATEADD(minute, r.ArrM, CAST(r.DateUTC AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- ===== 31/05/2026 (VN) =====
    -- HAN -> SGN: 1h55, 2h05, 2h00
    ('VN14001', 'Ha Noi', 'Ho Chi Minh', '2026-05-30',  -60,   55, 'Scheduled', 'VN'),
    ('VJ14002', 'Ha Noi', 'Ho Chi Minh', '2026-05-30',  240,  365, 'Scheduled', 'VJ'),
    ('QH14003', 'Ha Noi', 'Ho Chi Minh', '2026-05-30',  540,  660, 'Scheduled', 'QH'),
    -- SGN -> HAN: 2h00, 1h55, 2h05
    ('BL14004', 'Ho Chi Minh', 'Ha Noi', '2026-05-30',   60,  180, 'Scheduled', 'BL'),
    ('VN14005', 'Ho Chi Minh', 'Ha Noi', '2026-05-30',  360,  475, 'Scheduled', 'VN'),
    ('VJ14006', 'Ho Chi Minh', 'Ha Noi', '2026-05-30',  720,  845, 'Scheduled', 'VJ'),
    -- DAD -> SGN: 1h10, 1h20, 1h15
    ('QH14007', 'Da Nang', 'Ho Chi Minh', '2026-05-30',    0,   70, 'Scheduled', 'QH'),
    ('BL14008', 'Da Nang', 'Ho Chi Minh', '2026-05-30',  360,  440, 'Scheduled', 'BL'),
    ('VN14009', 'Da Nang', 'Ho Chi Minh', '2026-05-30',  660,  735, 'Scheduled', 'VN'),
    -- SGN -> DAD: 1h20, 1h25, 1h15
    ('VJ14010', 'Ho Chi Minh', 'Da Nang', '2026-05-30',  120,  200, 'Scheduled', 'VJ'),
    ('QH14011', 'Ho Chi Minh', 'Da Nang', '2026-05-30',  480,  565, 'Scheduled', 'QH'),
    ('BL14012', 'Ho Chi Minh', 'Da Nang', '2026-05-30',  780,  855, 'Scheduled', 'BL'),
    -- HAN -> DAD: 1h10, 1h15, 1h20
    ('VN14013', 'Ha Noi', 'Da Nang', '2026-05-30',  -30,   40, 'Scheduled', 'VN'),
    ('VJ14014', 'Ha Noi', 'Da Nang', '2026-05-30',  300,  375, 'Scheduled', 'VJ'),
    ('QH14015', 'Ha Noi', 'Da Nang', '2026-05-30',  630,  710, 'Scheduled', 'QH'),
    -- DAD -> HAN: 1h20, 1h10, 1h15
    ('BL14016', 'Da Nang', 'Ha Noi', '2026-05-30',  150,  230, 'Scheduled', 'BL'),
    ('VN14017', 'Da Nang', 'Ha Noi', '2026-05-30',  510,  580, 'Scheduled', 'VN'),
    ('VJ14018', 'Da Nang', 'Ha Noi', '2026-05-30',  780,  855, 'Scheduled', 'VJ'),
    -- SGN -> PQC: 0h55, 1h05, 1h00
    ('QH14019', 'Ho Chi Minh', 'Phu Quoc', '2026-05-30',   30,   85, 'Scheduled', 'QH'),
    ('BL14020', 'Ho Chi Minh', 'Phu Quoc', '2026-05-30',  390,  455, 'Scheduled', 'BL'),
    ('VN14021', 'Ho Chi Minh', 'Phu Quoc', '2026-05-30',  690,  750, 'Scheduled', 'VN'),
    -- PQC -> SGN: 1h00, 0h55, 1h05
    ('VJ14022', 'Phu Quoc', 'Ho Chi Minh', '2026-05-30',  180,  240, 'Scheduled', 'VJ'),
    ('QH14023', 'Phu Quoc', 'Ho Chi Minh', '2026-05-30',  540,  595, 'Scheduled', 'QH'),
    ('BL14024', 'Phu Quoc', 'Ho Chi Minh', '2026-05-30',  810,  875, 'Scheduled', 'BL'),
    -- SGN -> SIN: 1h55, 2h05
    ('SQ14025', 'Ho Chi Minh', 'Singapore', '2026-05-30',   60,  175, 'Scheduled', 'SQ'),
    ('VJ14026', 'Ho Chi Minh', 'Singapore', '2026-05-30',  480,  605, 'Scheduled', 'VJ'),
    -- HAN -> BKK: 2h00, 1h55
    ('TG14027', 'Ha Noi', 'Bangkok', '2026-05-30',  120,  240, 'Scheduled', 'TG'),
    ('VN14028', 'Ha Noi', 'Bangkok', '2026-05-30',  660,  775, 'Scheduled', 'VN'),

    -- ===== 01/06/2026 (VN) =====
    -- HAN -> SGN: 2h00, 1h55, 2h05
    ('VN15001', 'Ha Noi', 'Ho Chi Minh', '2026-05-31',  -60,   60, 'Scheduled', 'VN'),
    ('VJ15002', 'Ha Noi', 'Ho Chi Minh', '2026-05-31',  240,  355, 'Scheduled', 'VJ'),
    ('QH15003', 'Ha Noi', 'Ho Chi Minh', '2026-05-31',  600,  725, 'Scheduled', 'QH'),
    -- SGN -> HAN: 1h55, 2h05, 2h00
    ('BL15004', 'Ho Chi Minh', 'Ha Noi', '2026-05-31',   60,  175, 'Scheduled', 'BL'),
    ('VN15005', 'Ho Chi Minh', 'Ha Noi', '2026-05-31',  420,  545, 'Scheduled', 'VN'),
    ('VJ15006', 'Ho Chi Minh', 'Ha Noi', '2026-05-31',  780,  900, 'Scheduled', 'VJ'),
    -- DAD -> SGN: 1h15, 1h10, 1h20
    ('QH15007', 'Da Nang', 'Ho Chi Minh', '2026-05-31',    0,   75, 'Scheduled', 'QH'),
    ('BL15008', 'Da Nang', 'Ho Chi Minh', '2026-05-31',  360,  430, 'Scheduled', 'BL'),
    ('VN15009', 'Da Nang', 'Ho Chi Minh', '2026-05-31',  690,  770, 'Scheduled', 'VN'),
    -- SGN -> DAD: 1h15, 1h20, 1h25
    ('VJ15010', 'Ho Chi Minh', 'Da Nang', '2026-05-31',  150,  225, 'Scheduled', 'VJ'),
    ('QH15011', 'Ho Chi Minh', 'Da Nang', '2026-05-31',  480,  560, 'Scheduled', 'QH'),
    ('BL15012', 'Ho Chi Minh', 'Da Nang', '2026-05-31',  780,  865, 'Scheduled', 'BL'),
    -- HAN -> DAD: 1h20, 1h10, 1h15
    ('VN15013', 'Ha Noi', 'Da Nang', '2026-05-31',  -30,   50, 'Scheduled', 'VN'),
    ('VJ15014', 'Ha Noi', 'Da Nang', '2026-05-31',  270,  340, 'Scheduled', 'VJ'),
    ('QH15015', 'Ha Noi', 'Da Nang', '2026-05-31',  600,  675, 'Scheduled', 'QH'),
    -- DAD -> HAN: 1h15, 1h20, 1h10
    ('BL15016', 'Da Nang', 'Ha Noi', '2026-05-31',  120,  195, 'Scheduled', 'BL'),
    ('VN15017', 'Da Nang', 'Ha Noi', '2026-05-31',  480,  560, 'Scheduled', 'VN'),
    ('VJ15018', 'Da Nang', 'Ha Noi', '2026-05-31',  750,  820, 'Scheduled', 'VJ'),
    -- SGN -> PQC: 1h00, 0h55, 1h05
    ('QH15019', 'Ho Chi Minh', 'Phu Quoc', '2026-05-31',   60,  120, 'Scheduled', 'QH'),
    ('BL15020', 'Ho Chi Minh', 'Phu Quoc', '2026-05-31',  420,  475, 'Scheduled', 'BL'),
    ('VN15021', 'Ho Chi Minh', 'Phu Quoc', '2026-05-31',  720,  785, 'Scheduled', 'VN'),
    -- PQC -> SGN: 0h55, 1h05, 1h00
    ('VJ15022', 'Phu Quoc', 'Ho Chi Minh', '2026-05-31',  210,  265, 'Scheduled', 'VJ'),
    ('QH15023', 'Phu Quoc', 'Ho Chi Minh', '2026-05-31',  540,  605, 'Scheduled', 'QH'),
    ('BL15024', 'Phu Quoc', 'Ho Chi Minh', '2026-05-31',  810,  870, 'Scheduled', 'BL'),
    -- SGN -> SIN: 2h05, 1h55
    ('SQ15025', 'Ho Chi Minh', 'Singapore', '2026-05-31',   90,  215, 'Scheduled', 'SQ'),
    ('VJ15026', 'Ho Chi Minh', 'Singapore', '2026-05-31',  540,  655, 'Scheduled', 'VJ'),
    -- HAN -> BKK: 1h55, 2h05
    ('TG15027', 'Ha Noi', 'Bangkok', '2026-05-31',  180,  295, 'Scheduled', 'TG'),
    ('VN15028', 'Ha Noi', 'Bangkok', '2026-05-31',  660,  785, 'Scheduled', 'VN')
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
    ('VN14001', 1600000), ('VJ14002', 1400000), ('QH14003', 1500000),
    ('BL14004', 1550000), ('VN14005', 1650000), ('VJ14006', 1450000),
    ('QH14007',  950000), ('BL14008',  900000), ('VN14009', 1000000),
    ('VJ14010', 1050000), ('QH14011', 1100000), ('BL14012', 1000000),
    ('VN14013', 1200000), ('VJ14014', 1150000), ('QH14015', 1250000),
    ('BL14016', 1100000), ('VN14017', 1200000), ('VJ14018', 1150000),
    ('QH14019',  850000), ('BL14020',  800000), ('VN14021',  900000),
    ('VJ14022',  880000), ('QH14023',  820000), ('BL14024',  860000),
    ('SQ14025', 3200000), ('VJ14026', 2800000),
    ('TG14027', 2600000), ('VN14028', 2500000),
    ('VN15001', 1620000), ('VJ15002', 1420000), ('QH15003', 1520000),
    ('BL15004', 1570000), ('VN15005', 1670000), ('VJ15006', 1470000),
    ('QH15007',  970000), ('BL15008',  920000), ('VN15009', 1020000),
    ('VJ15010', 1070000), ('QH15011', 1120000), ('BL15012', 1020000),
    ('VN15013', 1220000), ('VJ15014', 1170000), ('QH15015', 1270000),
    ('BL15016', 1120000), ('VN15017', 1220000), ('VJ15018', 1170000),
    ('QH15019',  870000), ('BL15020',  820000), ('VN15021',  920000),
    ('VJ15022',  900000), ('QH15023',  840000), ('BL15024',  880000),
    ('SQ15025', 3300000), ('VJ15026', 2850000),
    ('TG15027', 2650000), ('VN15028', 2550000)
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
    ('VN14001', 3600000), ('VJ14002', 3200000), ('QH14003', 3400000),
    ('BL14004', 3500000), ('VN14005', 3700000), ('VJ14006', 3300000),
    ('QH14007', 2200000), ('BL14008', 2100000), ('VN14009', 2400000),
    ('VJ14010', 2500000), ('QH14011', 2600000), ('BL14012', 2400000),
    ('VN14013', 2800000), ('VJ14014', 2600000), ('QH14015', 2900000),
    ('BL14016', 2600000), ('VN14017', 2800000), ('VJ14018', 2700000),
    ('QH14019', 2000000), ('BL14020', 1900000), ('VN14021', 2100000),
    ('VJ14022', 2000000), ('QH14023', 1900000), ('BL14024', 2000000),
    ('SQ14025', 7000000), ('VJ14026', 6200000),
    ('TG14027', 5800000), ('VN14028', 5500000),
    ('VN15001', 3650000), ('VJ15002', 3250000), ('QH15003', 3450000),
    ('BL15004', 3550000), ('VN15005', 3750000), ('VJ15006', 3350000),
    ('QH15007', 2250000), ('BL15008', 2150000), ('VN15009', 2450000),
    ('VJ15010', 2550000), ('QH15011', 2650000), ('BL15012', 2450000),
    ('VN15013', 2850000), ('VJ15014', 2650000), ('QH15015', 2950000),
    ('BL15016', 2650000), ('VN15017', 2850000), ('VJ15018', 2750000),
    ('QH15019', 2050000), ('BL15020', 1950000), ('VN15021', 2150000),
    ('VJ15022', 2050000), ('QH15023', 1950000), ('BL15024', 2050000),
    ('SQ15025', 7200000), ('VJ15026', 6300000),
    ('TG15027', 5900000), ('VN15028', 5600000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats
            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE f.FlightNumber IN (
        'VN14001','VJ14002','QH14003','BL14004','VN14005','VJ14006',
        'QH14007','BL14008','VN14009','VJ14010','QH14011','BL14012',
        'VN14013','VJ14014','QH14015','BL14016','VN14017','VJ14018',
        'QH14019','BL14020','VN14021','VJ14022','QH14023','BL14024',
        'SQ14025','VJ14026','TG14027','VN14028',
        'VN15001','VJ15002','QH15003','BL15004','VN15005','VJ15006',
        'QH15007','BL15008','VN15009','VJ15010','QH15011','BL15012',
        'VN15013','VJ15014','QH15015','BL15016','VN15017','VJ15018',
        'QH15019','BL15020','VN15021','VJ15022','QH15023','BL15024',
        'SQ15025','VJ15026','TG15027','VN15028'
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
        'VN14001','VJ14002','QH14003','BL14004','VN14005','VJ14006',
        'QH14007','BL14008','VN14009','VJ14010','QH14011','BL14012',
        'VN14013','VJ14014','QH14015','BL14016','VN14017','VJ14018',
        'QH14019','BL14020','VN14021','VJ14022','QH14023','BL14024',
        'SQ14025','VJ14026','TG14027','VN14028',
        'VN15001','VJ15002','QH15003','BL15004','VN15005','VJ15006',
        'QH15007','BL15008','VN15009','VJ15010','QH15011','BL15012',
        'VN15013','VJ15014','QH15015','BL15016','VN15017','VJ15018',
        'QH15019','BL15020','VN15021','VJ15022','QH15023','BL15024',
        'SQ15025','VJ15026','TG15027','VN15028'
    )
);
DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE FlightNumber IN (
        'VN14001','VJ14002','QH14003','BL14004','VN14005','VJ14006',
        'QH14007','BL14008','VN14009','VJ14010','QH14011','BL14012',
        'VN14013','VJ14014','QH14015','BL14016','VN14017','VJ14018',
        'QH14019','BL14020','VN14021','VJ14022','QH14023','BL14024',
        'SQ14025','VJ14026','TG14027','VN14028',
        'VN15001','VJ15002','QH15003','BL15004','VN15005','VJ15006',
        'QH15007','BL15008','VN15009','VJ15010','QH15011','BL15012',
        'VN15013','VJ15014','QH15015','BL15016','VN15017','VJ15018',
        'QH15019','BL15020','VN15021','VJ15022','QH15023','BL15024',
        'SQ15025','VJ15026','TG15027','VN15028'
    )
);
DELETE FROM Flights WHERE FlightNumber IN (
    'VN14001','VJ14002','QH14003','BL14004','VN14005','VJ14006',
    'QH14007','BL14008','VN14009','VJ14010','QH14011','BL14012',
    'VN14013','VJ14014','QH14015','BL14016','VN14017','VJ14018',
    'QH14019','BL14020','VN14021','VJ14022','QH14023','BL14024',
    'SQ14025','VJ14026','TG14027','VN14028',
    'VN15001','VJ15002','QH15003','BL15004','VN15005','VJ15006',
    'QH15007','BL15008','VN15009','VJ15010','QH15011','BL15012',
    'VN15013','VJ15014','QH15015','BL15016','VN15017','VJ15018',
    'QH15019','BL15020','VN15021','VJ15022','QH15023','BL15024',
    'SQ15025','VJ15026','TG15027','VN15028'
);
");
        }
    }
}
