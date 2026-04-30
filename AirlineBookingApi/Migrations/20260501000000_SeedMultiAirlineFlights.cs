using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    public partial class SeedMultiAirlineFlights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Update AirlineId for existing flights based on flight number prefix
UPDATE f SET f.AirlineId = a.Id
FROM Flights f
JOIN Airlines a ON UPPER(LEFT(f.FlightNumber, 2)) = UPPER(a.Code)
WHERE f.AirlineId IS NULL;

-- Seed more flights from different airlines (14 days from now)
DECLARE @futureDate DATE = CONVERT(date, DATEADD(day, 14, GETUTCDATE()));

INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(@futureDate AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(@futureDate AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    ('VN1101', 'Ha Noi', 'Ho Chi Minh', 6, 8, 'Scheduled', 'VN'),
    ('VN1102', 'Ho Chi Minh', 'Ha Noi', 7, 9, 'Scheduled', 'VN'),
    ('VN1201', 'Da Nang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'VN'),
    ('VN1202', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VN'),
    ('VN1301', 'Da Nang', 'Ha Noi', 14, 16, 'Scheduled', 'VN'),
    ('VN1302', 'Ha Noi', 'Da Nang', 15, 17, 'Scheduled', 'VN'),
    ('VJ201', 'Ha Noi', 'Ho Chi Minh', 5, 7, 'Scheduled', 'VJ'),
    ('VJ202', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VJ'),
    ('VJ301', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    ('VJ302', 'Ho Chi Minh', 'Da Nang', 11, 12, 'Scheduled', 'VJ'),
    ('VJ401', 'Ha Noi', 'Da Nang', 16, 17, 'Scheduled', 'VJ'),
    ('VJ402', 'Da Nang', 'Ha Noi', 18, 19, 'Scheduled', 'VJ'),
    ('QH101', 'Ha Noi', 'Ho Chi Minh', 6, 8, 'Scheduled', 'QH'),
    ('QH102', 'Ho Chi Minh', 'Ha Noi', 9, 11, 'Scheduled', 'QH'),
    ('QH201', 'Da Nang', 'Ho Chi Minh', 7, 8, 'Scheduled', 'QH'),
    ('QH202', 'Ho Chi Minh', 'Da Nang', 12, 13, 'Scheduled', 'QH'),
    ('QH301', 'Da Nang', 'Ha Noi', 13, 15, 'Scheduled', 'QH'),
    ('QH302', 'Ha Noi', 'Da Nang', 17, 19, 'Scheduled', 'QH'),
    ('BL501', 'Ha Noi', 'Ho Chi Minh', 5, 7, 'Scheduled', 'BL'),
    ('BL502', 'Ho Chi Minh', 'Ha Noi', 8, 10, 'Scheduled', 'BL'),
    ('BL601', 'Da Nang', 'Ho Chi Minh', 10, 11, 'Scheduled', 'BL'),
    ('BL602', 'Ho Chi Minh', 'Da Nang', 14, 15, 'Scheduled', 'BL'),
    ('SQ701', 'Ho Chi Minh', 'Singapore', 8, 12, 'Scheduled', 'SQ'),
    ('SQ702', 'Singapore', 'Ho Chi Minh', 13, 15, 'Scheduled', 'SQ'),
    ('SQ703', 'Ha Noi', 'Singapore', 10, 14, 'Scheduled', 'SQ'),
    ('SQ704', 'Singapore', 'Ha Noi', 15, 19, 'Scheduled', 'SQ'),
    ('TG801', 'Ha Noi', 'Bangkok', 9, 12, 'Scheduled', 'TG'),
    ('TG802', 'Bangkok', 'Ha Noi', 13, 16, 'Scheduled', 'TG'),
    ('TG803', 'Ho Chi Minh', 'Bangkok', 7, 9, 'Scheduled', 'TG'),
    ('TG804', 'Bangkok', 'Ho Chi Minh', 10, 12, 'Scheduled', 'TG')
) r(FlightNum, Dep, Arr, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate);

-- Insert FlightPrices for new flights
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN1101', 1800000, 4200000), ('VN1102', 1800000, 4200000),
    ('VN1201', 1500000, 3500000), ('VN1202', 1500000, 3500000),
    ('VN1301', 1600000, 3800000), ('VN1302', 1600000, 3800000),
    ('VJ201', 1200000, 2800000), ('VJ202', 1200000, 2800000),
    ('VJ301', 900000, 2200000), ('VJ302', 900000, 2200000),
    ('VJ401', 1000000, 2400000), ('VJ402', 1000000, 2400000),
    ('QH101', 1500000, 3500000), ('QH102', 1500000, 3500000),
    ('QH201', 1100000, 2600000), ('QH202', 1100000, 2600000),
    ('QH301', 1300000, 3000000), ('QH302', 1300000, 3000000),
    ('BL501', 1000000, 2400000), ('BL502', 1000000, 2400000),
    ('BL601', 800000, 2000000), ('BL602', 800000, 2000000),
    ('SQ701', 3500000, 8000000), ('SQ702', 3500000, 8000000),
    ('SQ703', 3800000, 8500000), ('SQ704', 3800000, 8500000),
    ('TG801', 2800000, 6500000), ('TG802', 2800000, 6500000),
    ('TG803', 2600000, 6000000), ('TG804', 2600000, 6000000)
) r(FlightNum, EconPrice, BizPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN1101', 1800000, 4200000), ('VN1102', 1800000, 4200000),
    ('VN1201', 1500000, 3500000), ('VN1202', 1500000, 3500000),
    ('VN1301', 1600000, 3800000), ('VN1302', 1600000, 3800000),
    ('VJ201', 1200000, 2800000), ('VJ202', 1200000, 2800000),
    ('VJ301', 900000, 2200000), ('VJ302', 900000, 2200000),
    ('VJ401', 1000000, 2400000), ('VJ402', 1000000, 2400000),
    ('QH101', 1500000, 3500000), ('QH102', 1500000, 3500000),
    ('QH201', 1100000, 2600000), ('QH202', 1100000, 2600000),
    ('QH301', 1300000, 3000000), ('QH302', 1300000, 3000000),
    ('BL501', 1000000, 2400000), ('BL502', 1000000, 2400000),
    ('BL601', 800000, 2000000), ('BL602', 800000, 2000000),
    ('SQ701', 3500000, 8000000), ('SQ702', 3500000, 8000000),
    ('SQ703', 3800000, 8500000), ('SQ704', 3800000, 8500000),
    ('TG801', 2800000, 6500000), ('TG802', 2800000, 6500000),
    ('TG803', 2600000, 6000000), ('TG804', 2600000, 6000000)
) r(FlightNum, EconPrice, BizPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            migrationBuilder.Sql(@"
DECLARE @futureDate DATE = CONVERT(date, DATEADD(day, 14, GETUTCDATE()));

-- Insert 68 seats for new flights (50 Economy + 18 Business)
DECLARE @newFlightId INT;
DECLARE flightCursor CURSOR FOR
    SELECT f.Id FROM Flights f
    WHERE CONVERT(date, f.DepartureTime) = @futureDate
    AND NOT EXISTS (SELECT 1 FROM Seats s WHERE s.FlightId = f.Id);

OPEN flightCursor;
FETCH NEXT FROM flightCursor INTO @newFlightId;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= 50
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@newFlightId, CONCAT('E', RIGHT('0' + CAST(@i AS NVARCHAR(2)), 2)), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
        SET @i = @i + 1;
    END;
    SET @i = 1;
    WHILE @i <= 18
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@newFlightId, CONCAT('B', RIGHT('0' + CAST(@i AS NVARCHAR(2)), 2)), 'Business', 1, GETUTCDATE(), GETUTCDATE());
        SET @i = @i + 1;
    END;
    FETCH NEXT FROM flightCursor INTO @newFlightId;
END;
CLOSE flightCursor;
DEALLOCATE flightCursor;
");

            migrationBuilder.Sql(@"
-- Seed additional routes 21 days from now
DECLARE @futureDate2 DATE = CONVERT(date, DATEADD(day, 21, GETUTCDATE()));

INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(@futureDate2 AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(@futureDate2 AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    ('VN2101', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VN'),
    ('VN2102', 'Ha Noi', 'Ho Chi Minh', 9, 11, 'Scheduled', 'VN'),
    ('VJ501', 'Da Nang', 'Ha Noi', 7, 9, 'Scheduled', 'VJ'),
    ('VJ502', 'Ha Noi', 'Da Nang', 10, 12, 'Scheduled', 'VJ'),
    ('QH401', 'Da Nang', 'Ha Noi', 8, 10, 'Scheduled', 'QH'),
    ('QH402', 'Ha Noi', 'Da Nang', 14, 16, 'Scheduled', 'QH'),
    ('BL701', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'BL'),
    ('BL702', 'Da Nang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'BL')
) r(FlightNum, Dep, Arr, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate2);

-- Insert FlightPrices for second batch
INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.Econ, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN2101', 1800000, 4200000), ('VN2102', 1800000, 4200000),
    ('VJ501', 1000000, 2400000), ('VJ502', 1000000, 2400000),
    ('QH401', 1300000, 3000000), ('QH402', 1300000, 3000000),
    ('BL701', 800000, 2000000), ('BL702', 800000, 2000000)
) r(FlightNum, Econ, Biz)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate2
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.Biz, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    ('VN2101', 1800000, 4200000), ('VN2102', 1800000, 4200000),
    ('VJ501', 1000000, 2400000), ('VJ502', 1000000, 2400000),
    ('QH401', 1300000, 3000000), ('QH402', 1300000, 3000000),
    ('BL701', 800000, 2000000), ('BL702', 800000, 2000000)
) r(FlightNum, Econ, Biz)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @futureDate2
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');

-- Insert seats for second batch
DECLARE @flightId2 INT;
DECLARE fc2 CURSOR FOR
    SELECT f.Id FROM Flights f
    WHERE CONVERT(date, f.DepartureTime) = @futureDate2
    AND NOT EXISTS (SELECT 1 FROM Seats s WHERE s.FlightId = f.Id);

OPEN fc2;
FETCH NEXT FROM fc2 INTO @flightId2;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @ni INT = 1;
    WHILE @ni <= 50
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@flightId2, CONCAT('E', RIGHT('0' + CAST(@ni AS NVARCHAR(2)), 2)), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
        SET @ni = @ni + 1;
    END;
    SET @ni = 1;
    WHILE @ni <= 18
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@flightId2, CONCAT('B', RIGHT('0' + CAST(@ni AS NVARCHAR(2)), 2)), 'Business', 1, GETUTCDATE(), GETUTCDATE());
        SET @ni = @ni + 1;
    END;
    FETCH NEXT FROM fc2 INTO @flightId2;
END;
CLOSE fc2;
DEALLOCATE fc2;
");

            migrationBuilder.Sql(@"
-- Update seats for existing demo flights (VN901, VJ321, etc.) to have 68 seats
DECLARE @demoFlights TABLE(Id INT);
INSERT INTO @demoFlights SELECT Id FROM Flights WHERE FlightNumber IN ('VN901','VJ321','QH205','BL442','SQ177','TG551','VJ888','VN777');
DELETE FROM Seats WHERE FlightId IN (SELECT Id FROM @demoFlights);

DECLARE @dfId INT;
DECLARE dfCursor CURSOR FOR SELECT Id FROM @demoFlights;
OPEN dfCursor;
FETCH NEXT FROM dfCursor INTO @dfId;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= 50
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@dfId, CONCAT('E', RIGHT('0' + CAST(@i AS NVARCHAR(2)), 2)), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
        SET @i = @i + 1;
    END;
    SET @i = 1;
    WHILE @i <= 18
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@dfId, CONCAT('B', RIGHT('0' + CAST(@i AS NVARCHAR(2)), 2)), 'Business', 1, GETUTCDATE(), GETUTCDATE());
        SET @i = @i + 1;
    END;
    FETCH NEXT FROM dfCursor INTO @dfId;
END;
CLOSE dfCursor;
DEALLOCATE dfCursor;

-- Update seats for original seed flights (VN1001, VN1002, VN2001, VN3001) to have 68 seats
DECLARE @seedFlights TABLE(Id INT);
INSERT INTO @seedFlights SELECT Id FROM Flights WHERE FlightNumber IN ('VN1001','VN1002','VN2001','VN3001');
DELETE FROM Seats WHERE FlightId IN (SELECT Id FROM @seedFlights);

DECLARE @sfId INT;
DECLARE sfCursor CURSOR FOR SELECT Id FROM @seedFlights;
OPEN sfCursor;
FETCH NEXT FROM sfCursor INTO @sfId;
WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @si INT = 1;
    WHILE @si <= 50
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@sfId, CONCAT('E', RIGHT('0' + CAST(@si AS NVARCHAR(2)), 2)), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
        SET @si = @si + 1;
    END;
    SET @si = 1;
    WHILE @si <= 18
    BEGIN
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@sfId, CONCAT('B', RIGHT('0' + CAST(@si AS NVARCHAR(2)), 2)), 'Business', 1, GETUTCDATE(), GETUTCDATE());
        SET @si = @si + 1;
    END;
    FETCH NEXT FROM sfCursor INTO @sfId;
END;
CLOSE sfCursor;
DEALLOCATE sfCursor;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
