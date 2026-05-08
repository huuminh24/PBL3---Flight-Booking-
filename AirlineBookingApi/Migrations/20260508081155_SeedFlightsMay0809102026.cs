using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlightsMay0809102026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Day 1: 08/05/2026 VN = 2026-05-07 UTC
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-07';

-- Insert Flights for Day 1 (08/05 VN) - Flight numbers: 6xxx
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(@date AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(@date AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- HAN -> SGN (06:00, 08:00, 10:00, 14:00 VN = -1, 1, 3, 7 UTC, 2h flight)
    ('VN6101', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VN'),
    ('VN6102', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VN'),
    ('VN6103', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VN'),
    ('VN6104', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VN'),
    ('VJ6201', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VJ'),
    ('VJ6202', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VJ'),
    ('VJ6203', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VJ'),
    ('VJ6204', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VJ'),
    ('QH6301', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'QH'),
    ('QH6302', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'QH'),
    ('QH6303', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'QH'),
    ('QH6304', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'QH'),
    ('BL6401', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'BL'),
    ('BL6402', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'BL'),
    ('BL6403', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'BL'),
    ('BL6404', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'BL'),
    -- SGN -> HAN (07:00, 09:00, 13:00, 17:00 VN = 0, 2, 6, 10 UTC, 2h flight)
    ('VN6111', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VN'),
    ('VN6112', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VN'),
    ('VN6113', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VN'),
    ('VN6114', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VN'),
    ('VJ6211', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VJ'),
    ('VJ6212', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VJ'),
    ('VJ6213', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VJ'),
    ('VJ6214', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    ('QH6311', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'QH'),
    ('QH6312', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'QH'),
    ('QH6313', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'QH'),
    ('QH6314', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'QH'),
    ('BL6411', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'BL'),
    ('BL6412', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'BL'),
    ('BL6413', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'BL'),
    ('BL6414', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'BL'),
    -- DAD -> SGN (07:00, 10:00, 15:00 VN = 0, 3, 8 UTC, 1h15 flight)
    ('VN6121', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VN'),
    ('VN6122', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VN'),
    ('VN6123', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ6221', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VJ'),
    ('VJ6222', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ6223', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    ('QH6321', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'QH'),
    ('QH6322', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH6323', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'QH'),
    -- SGN -> DAD (08:00, 12:00, 17:00 VN = 1, 5, 10 UTC, 1h15 flight)
    ('VN6131', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VN'),
    ('VN6132', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VN'),
    ('VN6133', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VN'),
    ('VJ6231', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ6232', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VJ'),
    ('VJ6233', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VJ'),
    ('QH6331', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'QH'),
    ('QH6332', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'QH'),
    ('QH6333', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'QH'),
    -- HAN -> DAD (06:30, 11:00 VN = -1+0.5= -0.5 -> use DATEADD(minute, -30), 4 UTC, 1h15 flight)
    ('VN6141', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VN'),
    ('VN6142', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VN'),
    ('VJ6241', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VJ'),
    ('VJ6242', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VJ'),
    -- DAD -> HAN (09:00, 16:00 VN = 2, 9 UTC, 1h15 flight)
    ('VN6151', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN6152', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VN'),
    ('VJ6251', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VJ'),
    ('VJ6252', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VJ'),
    -- SGN -> PQC (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN6161', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VN'),
    ('VN6162', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VN'),
    ('VJ6261', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VJ'),
    ('VJ6262', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VJ'),
    -- PQC -> SGN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN6171', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VN'),
    ('VN6172', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ6271', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    ('VJ6272', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    -- SGN -> NTR (08:00, 14:00 VN = 1, 7 UTC, 1h flight)
    ('VJ6281', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ6282', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'VJ'),
    ('QH6381', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'QH'),
    ('QH6382', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'QH'),
    -- NTR -> SGN (10:00, 16:00 VN = 3, 9 UTC, 1h flight)
    ('VJ6291', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ6292', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'VJ'),
    ('QH6391', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH6392', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'QH'),
    -- HAN -> PQC (07:00, 12:00 VN = 0, 5 UTC, 2h15 flight)
    ('VN6181', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'VN'),
    ('VN6182', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'VN'),
    ('QH6481', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'QH'),
    ('QH6482', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'QH'),
    -- PQC -> HAN (10:00, 15:00 VN = 3, 8 UTC, 2h15 flight)
    ('VN6191', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'VN'),
    ('VN6192', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'VN'),
    ('QH6491', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'QH'),
    ('QH6492', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'QH'),
    -- SGN -> VCA (08:00 VN = 1 UTC, 0h45 flight)
    ('VJ62A1', 'Ho Chi Minh', 'Can Tho', 1, 2, 'Scheduled', 'VJ'),
    -- VCA -> SGN (09:30 VN = 2 UTC approx, 0h45 flight)
    ('VJ62B1', 'Can Tho', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    -- HAN -> HUI (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN61A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'VN'),
    ('VN61A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'VN'),
    ('BL64A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'BL'),
    ('BL64A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'BL'),
    -- HUI -> HAN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN61B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN61B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'VN'),
    ('BL64B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'BL'),
    ('BL64B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'BL'),
    -- HAN -> HPH (08:00 VN = 1 UTC, 0h30 flight)
    ('VJ62C1', 'Ha Noi', 'Hai Phong', 1, 1, 'Scheduled', 'VJ'),
    -- HPH -> HAN (09:00 VN = 2 UTC, 0h30 flight)
    ('VJ62D1', 'Hai Phong', 'Ha Noi', 2, 2, 'Scheduled', 'VJ'),
    -- SGN -> SIN (08:00, 11:00 VN = 1, 4 UTC, 2h flight)
    ('SQ6701', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'SQ'),
    ('SQ6702', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'SQ'),
    ('VJ62E1', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'VJ'),
    ('VJ62E2', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'VJ'),
    -- SIN -> SGN (13:00, 16:00 VN = 6, 9 UTC, 2h flight)
    ('SQ6711', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'SQ'),
    ('SQ6712', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'SQ'),
    ('VJ62F1', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'VJ'),
    ('VJ62F2', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'VJ'),
    -- HAN -> BKK (09:00, 14:00 VN = 2, 7 UTC, 2h flight)
    ('TG6801', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'TG'),
    ('TG6802', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'TG'),
    ('VJ62G1', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'VJ'),
    ('VJ62G2', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'VJ'),
    -- BKK -> HAN (12:00, 17:00 VN = 5, 10 UTC, 2h flight)
    ('TG6811', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'TG'),
    ('TG6812', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'TG'),
    ('VJ62H1', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'VJ'),
    ('VJ62H2', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    -- SGN -> KUL (09:00 VN = 2 UTC, 2h flight)
    ('VJ62I1', 'Ho Chi Minh', 'Kuala Lumpur', 2, 4, 'Scheduled', 'VJ'),
    -- KUL -> SGN (12:30 VN = 5.5 UTC, 2h flight)
    ('VJ62J1', 'Kuala Lumpur', 'Ho Chi Minh', 5, 7, 'Scheduled', 'VJ'),
    -- HAN -> TYO (10:00 VN = 3 UTC, 5h flight)
    ('VN61C1', 'Ha Noi', 'Tokyo', 3, 8, 'Scheduled', 'VN'),
    -- TYO -> HAN (16:00 VN = 9 UTC, 5h flight)
    ('VN61D1', 'Tokyo', 'Ha Noi', 9, 14, 'Scheduled', 'VN'),
    -- SGN -> SEL (08:00, 22:00 VN = 1, 15 UTC, 5h flight)
    ('VN61E1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VN'),
    ('VN61E2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VN'),
    ('VJ62K1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VJ'),
    ('VJ62K2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VJ'),
    -- SEL -> SGN (14:00, 06:00+1 VN = 7, -1 UTC, 5h flight)
    ('VN61F1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VN'),
    ('VN61F2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VN'),
    ('VJ62L1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VJ'),
    ('VJ62L2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VJ')
) r(FlightNum, Dep, Arr, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date);
");

            // Insert FlightPrices for Day 1
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-07';

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 1,500,000 – 1,800,000
    ('VN6101', 1500000), ('VN6102', 1600000), ('VN6103', 1700000), ('VN6104', 1800000),
    ('VJ6201', 1500000), ('VJ6202', 1600000), ('VJ6203', 1700000), ('VJ6204', 1800000),
    ('QH6301', 1500000), ('QH6302', 1600000), ('QH6303', 1700000), ('QH6304', 1800000),
    ('BL6401', 1500000), ('BL6402', 1600000), ('BL6403', 1700000), ('BL6404', 1800000),
    ('VN6111', 1500000), ('VN6112', 1600000), ('VN6113', 1700000), ('VN6114', 1800000),
    ('VJ6211', 1500000), ('VJ6212', 1600000), ('VJ6213', 1700000), ('VJ6214', 1800000),
    ('QH6311', 1500000), ('QH6312', 1600000), ('QH6313', 1700000), ('QH6314', 1800000),
    ('BL6411', 1500000), ('BL6412', 1600000), ('BL6413', 1700000), ('BL6414', 1800000),
    -- DAD <-> SGN: 900,000 – 1,100,000
    ('VN6121', 900000), ('VN6122', 1000000), ('VN6123', 1100000),
    ('VJ6221', 900000), ('VJ6222', 1000000), ('VJ6223', 1100000),
    ('QH6321', 900000), ('QH6322', 1000000), ('QH6323', 1100000),
    ('VN6131', 900000), ('VN6132', 1000000), ('VN6133', 1100000),
    ('VJ6231', 900000), ('VJ6232', 1000000), ('VJ6233', 1100000),
    ('QH6331', 900000), ('QH6332', 1000000), ('QH6333', 1100000),
    -- HAN <-> DAD: 1,000,000 – 1,300,000
    ('VN6141', 1000000), ('VN6142', 1100000),
    ('VJ6241', 1000000), ('VJ6242', 1100000),
    ('VN6151', 1000000), ('VN6152', 1100000),
    ('VJ6251', 1000000), ('VJ6252', 1100000),
    -- SGN <-> PQC: 800,000 – 1,000,000
    ('VN6161', 800000), ('VN6162', 900000),
    ('VJ6261', 800000), ('VJ6262', 900000),
    ('VN6171', 800000), ('VN6172', 900000),
    ('VJ6271', 800000), ('VJ6272', 900000),
    -- SGN <-> NTR: 700,000 – 900,000
    ('VJ6281', 700000), ('VJ6282', 800000),
    ('QH6381', 700000), ('QH6382', 800000),
    ('VJ6291', 700000), ('VJ6292', 800000),
    ('QH6391', 700000), ('QH6392', 800000),
    -- HAN <-> PQC: 1,200,000 – 1,500,000
    ('VN6181', 1200000), ('VN6182', 1300000),
    ('QH6481', 1200000), ('QH6482', 1300000),
    ('VN6191', 1200000), ('VN6192', 1300000),
    ('QH6491', 1200000), ('QH6492', 1300000),
    -- SGN <-> VCA: 500,000 – 700,000
    ('VJ62A1', 500000), ('VJ62B1', 600000),
    -- HAN <-> HUI: 700,000 – 900,000
    ('VN61A1', 700000), ('VN61A2', 800000),
    ('BL64A1', 700000), ('BL64A2', 800000),
    ('VN61B1', 700000), ('VN61B2', 800000),
    ('BL64B1', 700000), ('BL64B2', 800000),
    -- HAN <-> HPH: 400,000 – 600,000
    ('VJ62C1', 400000), ('VJ62D1', 500000),
    -- SGN <-> SIN: 3,500,000
    ('SQ6701', 3500000), ('SQ6702', 3500000),
    ('SQ6711', 3500000), ('SQ6712', 3500000),
    ('VJ62E1', 3500000), ('VJ62E2', 3500000),
    ('VJ62F1', 3500000), ('VJ62F2', 3500000),
    -- HAN <-> BKK: 2,800,000
    ('TG6801', 2800000), ('TG6802', 2800000),
    ('TG6811', 2800000), ('TG6812', 2800000),
    ('VJ62G1', 2800000), ('VJ62G2', 2800000),
    ('VJ62H1', 2800000), ('VJ62H2', 2800000),
    -- SGN <-> KUL: 2,500,000
    ('VJ62I1', 2500000), ('VJ62J1', 2500000),
    -- HAN <-> TYO: 6,000,000
    ('VN61C1', 6000000), ('VN61D1', 6000000),
    -- SGN <-> SEL: 5,500,000
    ('VN61E1', 5500000), ('VN61E2', 5500000),
    ('VN61F1', 5500000), ('VN61F2', 5500000),
    ('VJ62K1', 5500000), ('VJ62K2', 5500000),
    ('VJ62L1', 5500000), ('VJ62L2', 5500000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 3,500,000 – 4,200,000
    ('VN6101', 3500000), ('VN6102', 3700000), ('VN6103', 3900000), ('VN6104', 4200000),
    ('VJ6201', 3500000), ('VJ6202', 3700000), ('VJ6203', 3900000), ('VJ6204', 4200000),
    ('QH6301', 3500000), ('QH6302', 3700000), ('QH6303', 3900000), ('QH6304', 4200000),
    ('BL6401', 3500000), ('BL6402', 3700000), ('BL6403', 3900000), ('BL6404', 4200000),
    ('VN6111', 3500000), ('VN6112', 3700000), ('VN6113', 3900000), ('VN6114', 4200000),
    ('VJ6211', 3500000), ('VJ6212', 3700000), ('VJ6213', 3900000), ('VJ6214', 4200000),
    ('QH6311', 3500000), ('QH6312', 3700000), ('QH6313', 3900000), ('QH6314', 4200000),
    ('BL6411', 3500000), ('BL6412', 3700000), ('BL6413', 3900000), ('BL6414', 4200000),
    -- DAD <-> SGN: 2,200,000 – 2,600,000
    ('VN6121', 2200000), ('VN6122', 2400000), ('VN6123', 2600000),
    ('VJ6221', 2200000), ('VJ6222', 2400000), ('VJ6223', 2600000),
    ('QH6321', 2200000), ('QH6322', 2400000), ('QH6323', 2600000),
    ('VN6131', 2200000), ('VN6132', 2400000), ('VN6133', 2600000),
    ('VJ6231', 2200000), ('VJ6232', 2400000), ('VJ6233', 2600000),
    ('QH6331', 2200000), ('QH6332', 2400000), ('QH6333', 2600000),
    -- HAN <-> DAD: 2,400,000 – 3,000,000
    ('VN6141', 2400000), ('VN6142', 2600000),
    ('VJ6241', 2400000), ('VJ6242', 2600000),
    ('VN6151', 2400000), ('VN6152', 2600000),
    ('VJ6251', 2400000), ('VJ6252', 2600000),
    -- SGN <-> PQC: 2,000,000 – 2,400,000
    ('VN6161', 2000000), ('VN6162', 2200000),
    ('VJ6261', 2000000), ('VJ6262', 2200000),
    ('VN6171', 2000000), ('VN6172', 2200000),
    ('VJ6271', 2000000), ('VJ6272', 2200000),
    -- SGN <-> NTR: 1,800,000 – 2,200,000
    ('VJ6281', 1800000), ('VJ6282', 2000000),
    ('QH6381', 1800000), ('QH6382', 2000000),
    ('VJ6291', 1800000), ('VJ6292', 2000000),
    ('QH6391', 1800000), ('QH6392', 2000000),
    -- HAN <-> PQC: 2,800,000 – 3,500,000
    ('VN6181', 2800000), ('VN6182', 3000000),
    ('QH6481', 2800000), ('QH6482', 3000000),
    ('VN6191', 2800000), ('VN6192', 3000000),
    ('QH6491', 2800000), ('QH6492', 3000000),
    -- SGN <-> VCA: 1,200,000 – 1,600,000
    ('VJ62A1', 1200000), ('VJ62B1', 1400000),
    -- HAN <-> HUI: 1,800,000 – 2,200,000
    ('VN61A1', 1800000), ('VN61A2', 2000000),
    ('BL64A1', 1800000), ('BL64A2', 2000000),
    ('VN61B1', 1800000), ('VN61B2', 2000000),
    ('BL64B1', 1800000), ('BL64B2', 2000000),
    -- HAN <-> HPH: 1,000,000 – 1,400,000
    ('VJ62C1', 1000000), ('VJ62D1', 1200000),
    -- SGN <-> SIN: 8,000,000
    ('SQ6701', 8000000), ('SQ6702', 8000000),
    ('SQ6711', 8000000), ('SQ6712', 8000000),
    ('VJ62E1', 8000000), ('VJ62E2', 8000000),
    ('VJ62F1', 8000000), ('VJ62F2', 8000000),
    -- HAN <-> BKK: 6,500,000
    ('TG6801', 6500000), ('TG6802', 6500000),
    ('TG6811', 6500000), ('TG6812', 6500000),
    ('VJ62G1', 6500000), ('VJ62G2', 6500000),
    ('VJ62H1', 6500000), ('VJ62H2', 6500000),
    -- SGN <-> KUL: 6,000,000
    ('VJ62I1', 6000000), ('VJ62J1', 6000000),
    -- HAN <-> TYO: 14,000,000
    ('VN61C1', 14000000), ('VN61D1', 14000000),
    -- SGN <-> SEL: 13,000,000
    ('VN61E1', 13000000), ('VN61E2', 13000000),
    ('VN61F1', 13000000), ('VN61F2', 13000000),
    ('VJ62K1', 13000000), ('VJ62K2', 13000000),
    ('VJ62L1', 13000000), ('VJ62L2', 13000000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats for Day 1
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-07';
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE CONVERT(date, f.DepartureTime) = @date
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

            // Day 2: 09/05/2026 VN = 2026-05-08 UTC
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-08';

-- Insert Flights for Day 2 (09/05 VN) - Flight numbers: 7xxx
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(@date AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(@date AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- HAN -> SGN (06:00, 08:00, 10:00, 14:00 VN = -1, 1, 3, 7 UTC, 2h flight)
    ('VN7101', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VN'),
    ('VN7102', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VN'),
    ('VN7103', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VN'),
    ('VN7104', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VN'),
    ('VJ7201', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VJ'),
    ('VJ7202', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VJ'),
    ('VJ7203', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VJ'),
    ('VJ7204', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VJ'),
    ('QH7301', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'QH'),
    ('QH7302', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'QH'),
    ('QH7303', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'QH'),
    ('QH7304', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'QH'),
    ('BL7401', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'BL'),
    ('BL7402', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'BL'),
    ('BL7403', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'BL'),
    ('BL7404', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'BL'),
    -- SGN -> HAN (07:00, 09:00, 13:00, 17:00 VN = 0, 2, 6, 10 UTC, 2h flight)
    ('VN7111', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VN'),
    ('VN7112', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VN'),
    ('VN7113', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VN'),
    ('VN7114', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VN'),
    ('VJ7211', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VJ'),
    ('VJ7212', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VJ'),
    ('VJ7213', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VJ'),
    ('VJ7214', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    ('QH7311', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'QH'),
    ('QH7312', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'QH'),
    ('QH7313', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'QH'),
    ('QH7314', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'QH'),
    ('BL7411', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'BL'),
    ('BL7412', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'BL'),
    ('BL7413', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'BL'),
    ('BL7414', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'BL'),
    -- DAD -> SGN (07:00, 10:00, 15:00 VN = 0, 3, 8 UTC, 1h15 flight)
    ('VN7121', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VN'),
    ('VN7122', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VN'),
    ('VN7123', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ7221', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VJ'),
    ('VJ7222', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ7223', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    ('QH7321', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'QH'),
    ('QH7322', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH7323', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'QH'),
    -- SGN -> DAD (08:00, 12:00, 17:00 VN = 1, 5, 10 UTC, 1h15 flight)
    ('VN7131', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VN'),
    ('VN7132', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VN'),
    ('VN7133', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VN'),
    ('VJ7231', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ7232', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VJ'),
    ('VJ7233', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VJ'),
    ('QH7331', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'QH'),
    ('QH7332', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'QH'),
    ('QH7333', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'QH'),
    -- HAN -> DAD (06:30, 11:00 VN = -1, 4 UTC, 1h15 flight)
    ('VN7141', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VN'),
    ('VN7142', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VN'),
    ('VJ7241', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VJ'),
    ('VJ7242', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VJ'),
    -- DAD -> HAN (09:00, 16:00 VN = 2, 9 UTC, 1h15 flight)
    ('VN7151', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN7152', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VN'),
    ('VJ7251', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VJ'),
    ('VJ7252', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VJ'),
    -- SGN -> PQC (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN7161', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VN'),
    ('VN7162', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VN'),
    ('VJ7261', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VJ'),
    ('VJ7262', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VJ'),
    -- PQC -> SGN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN7171', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VN'),
    ('VN7172', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ7271', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    ('VJ7272', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    -- SGN -> NTR (08:00, 14:00 VN = 1, 7 UTC, 1h flight)
    ('VJ7281', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ7282', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'VJ'),
    ('QH7381', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'QH'),
    ('QH7382', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'QH'),
    -- NTR -> SGN (10:00, 16:00 VN = 3, 9 UTC, 1h flight)
    ('VJ7291', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ7292', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'VJ'),
    ('QH7391', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH7392', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'QH'),
    -- HAN -> PQC (07:00, 12:00 VN = 0, 5 UTC, 2h15 flight)
    ('VN7181', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'VN'),
    ('VN7182', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'VN'),
    ('QH7481', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'QH'),
    ('QH7482', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'QH'),
    -- PQC -> HAN (10:00, 15:00 VN = 3, 8 UTC, 2h15 flight)
    ('VN7191', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'VN'),
    ('VN7192', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'VN'),
    ('QH7491', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'QH'),
    ('QH7492', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'QH'),
    -- SGN -> VCA (08:00 VN = 1 UTC, 0h45 flight)
    ('VJ72A1', 'Ho Chi Minh', 'Can Tho', 1, 2, 'Scheduled', 'VJ'),
    -- VCA -> SGN (09:30 VN = 2 UTC approx, 0h45 flight)
    ('VJ72B1', 'Can Tho', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    -- HAN -> HUI (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN71A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'VN'),
    ('VN71A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'VN'),
    ('BL74A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'BL'),
    ('BL74A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'BL'),
    -- HUI -> HAN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN71B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN71B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'VN'),
    ('BL74B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'BL'),
    ('BL74B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'BL'),
    -- HAN -> HPH (08:00 VN = 1 UTC, 0h30 flight)
    ('VJ72C1', 'Ha Noi', 'Hai Phong', 1, 1, 'Scheduled', 'VJ'),
    -- HPH -> HAN (09:00 VN = 2 UTC, 0h30 flight)
    ('VJ72D1', 'Hai Phong', 'Ha Noi', 2, 2, 'Scheduled', 'VJ'),
    -- SGN -> SIN (08:00, 11:00 VN = 1, 4 UTC, 2h flight)
    ('SQ7701', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'SQ'),
    ('SQ7702', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'SQ'),
    ('VJ72E1', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'VJ'),
    ('VJ72E2', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'VJ'),
    -- SIN -> SGN (13:00, 16:00 VN = 6, 9 UTC, 2h flight)
    ('SQ7711', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'SQ'),
    ('SQ7712', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'SQ'),
    ('VJ72F1', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'VJ'),
    ('VJ72F2', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'VJ'),
    -- HAN -> BKK (09:00, 14:00 VN = 2, 7 UTC, 2h flight)
    ('TG7801', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'TG'),
    ('TG7802', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'TG'),
    ('VJ72G1', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'VJ'),
    ('VJ72G2', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'VJ'),
    -- BKK -> HAN (12:00, 17:00 VN = 5, 10 UTC, 2h flight)
    ('TG7811', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'TG'),
    ('TG7812', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'TG'),
    ('VJ72H1', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'VJ'),
    ('VJ72H2', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    -- SGN -> KUL (09:00 VN = 2 UTC, 2h flight)
    ('VJ72I1', 'Ho Chi Minh', 'Kuala Lumpur', 2, 4, 'Scheduled', 'VJ'),
    -- KUL -> SGN (12:30 VN = 2 UTC approx, 2h flight)
    ('VJ72J1', 'Kuala Lumpur', 'Ho Chi Minh', 2, 4, 'Scheduled', 'VJ'),
    -- HAN -> TYO (10:00 VN = 3 UTC, 5h flight)
    ('VN71C1', 'Ha Noi', 'Tokyo', 3, 8, 'Scheduled', 'VN'),
    -- TYO -> HAN (16:00 VN = 9 UTC, 5h flight)
    ('VN71D1', 'Tokyo', 'Ha Noi', 9, 14, 'Scheduled', 'VN'),
    -- SGN -> SEL (08:00, 22:00 VN = 1, 15 UTC, 5h flight)
    ('VN71E1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VN'),
    ('VN71E2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VN'),
    ('VJ72K1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VJ'),
    ('VJ72K2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VJ'),
    -- SEL -> SGN (14:00, 06:00+1 VN = 7, -1 UTC, 5h flight)
    ('VN71F1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VN'),
    ('VN71F2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VN'),
    ('VJ72L1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VJ'),
    ('VJ72L2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VJ')
) r(FlightNum, Dep, Arr, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date);
");

            // Insert FlightPrices for Day 2
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-08';

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 1,500,000 – 1,800,000
    ('VN7101', 1500000), ('VN7102', 1600000), ('VN7103', 1700000), ('VN7104', 1800000),
    ('VJ7201', 1500000), ('VJ7202', 1600000), ('VJ7203', 1700000), ('VJ7204', 1800000),
    ('QH7301', 1500000), ('QH7302', 1600000), ('QH7303', 1700000), ('QH7304', 1800000),
    ('BL7401', 1500000), ('BL7402', 1600000), ('BL7403', 1700000), ('BL7404', 1800000),
    ('VN7111', 1500000), ('VN7112', 1600000), ('VN7113', 1700000), ('VN7114', 1800000),
    ('VJ7211', 1500000), ('VJ7212', 1600000), ('VJ7213', 1700000), ('VJ7214', 1800000),
    ('QH7311', 1500000), ('QH7312', 1600000), ('QH7313', 1700000), ('QH7314', 1800000),
    ('BL7411', 1500000), ('BL7412', 1600000), ('BL7413', 1700000), ('BL7414', 1800000),
    -- DAD <-> SGN: 900,000 – 1,100,000
    ('VN7121', 900000), ('VN7122', 1000000), ('VN7123', 1100000),
    ('VJ7221', 900000), ('VJ7222', 1000000), ('VJ7223', 1100000),
    ('QH7321', 900000), ('QH7322', 1000000), ('QH7323', 1100000),
    ('VN7131', 900000), ('VN7132', 1000000), ('VN7133', 1100000),
    ('VJ7231', 900000), ('VJ7232', 1000000), ('VJ7233', 1100000),
    ('QH7331', 900000), ('QH7332', 1000000), ('QH7333', 1100000),
    -- HAN <-> DAD: 1,000,000 – 1,300,000
    ('VN7141', 1000000), ('VN7142', 1100000),
    ('VJ7241', 1000000), ('VJ7242', 1100000),
    ('VN7151', 1000000), ('VN7152', 1100000),
    ('VJ7251', 1000000), ('VJ7252', 1100000),
    -- SGN <-> PQC: 800,000 – 1,000,000
    ('VN7161', 800000), ('VN7162', 900000),
    ('VJ7261', 800000), ('VJ7262', 900000),
    ('VN7171', 800000), ('VN7172', 900000),
    ('VJ7271', 800000), ('VJ7272', 900000),
    -- SGN <-> NTR: 700,000 – 900,000
    ('VJ7281', 700000), ('VJ7282', 800000),
    ('QH7381', 700000), ('QH7382', 800000),
    ('VJ7291', 700000), ('VJ7292', 800000),
    ('QH7391', 700000), ('QH7392', 800000),
    -- HAN <-> PQC: 1,200,000 – 1,500,000
    ('VN7181', 1200000), ('VN7182', 1300000),
    ('QH7481', 1200000), ('QH7482', 1300000),
    ('VN7191', 1200000), ('VN7192', 1300000),
    ('QH7491', 1200000), ('QH7492', 1300000),
    -- SGN <-> VCA: 500,000 – 700,000
    ('VJ72A1', 500000), ('VJ72B1', 600000),
    -- HAN <-> HUI: 700,000 – 900,000
    ('VN71A1', 700000), ('VN71A2', 800000),
    ('BL74A1', 700000), ('BL74A2', 800000),
    ('VN71B1', 700000), ('VN71B2', 800000),
    ('BL74B1', 700000), ('BL74B2', 800000),
    -- HAN <-> HPH: 400,000 – 600,000
    ('VJ72C1', 400000), ('VJ72D1', 500000),
    -- SGN <-> SIN: 3,500,000
    ('SQ7701', 3500000), ('SQ7702', 3500000),
    ('SQ7711', 3500000), ('SQ7712', 3500000),
    ('VJ72E1', 3500000), ('VJ72E2', 3500000),
    ('VJ72F1', 3500000), ('VJ72F2', 3500000),
    -- HAN <-> BKK: 2,800,000
    ('TG7801', 2800000), ('TG7802', 2800000),
    ('TG7811', 2800000), ('TG7812', 2800000),
    ('VJ72G1', 2800000), ('VJ72G2', 2800000),
    ('VJ72H1', 2800000), ('VJ72H2', 2800000),
    -- SGN <-> KUL: 2,500,000
    ('VJ72I1', 2500000), ('VJ72J1', 2500000),
    -- HAN <-> TYO: 6,000,000
    ('VN71C1', 6000000), ('VN71D1', 6000000),
    -- SGN <-> SEL: 5,500,000
    ('VN71E1', 5500000), ('VN71E2', 5500000),
    ('VN71F1', 5500000), ('VN71F2', 5500000),
    ('VJ72K1', 5500000), ('VJ72K2', 5500000),
    ('VJ72L1', 5500000), ('VJ72L2', 5500000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 3,500,000 – 4,200,000
    ('VN7101', 3500000), ('VN7102', 3700000), ('VN7103', 3900000), ('VN7104', 4200000),
    ('VJ7201', 3500000), ('VJ7202', 3700000), ('VJ7203', 3900000), ('VJ7204', 4200000),
    ('QH7301', 3500000), ('QH7302', 3700000), ('QH7303', 3900000), ('QH7304', 4200000),
    ('BL7401', 3500000), ('BL7402', 3700000), ('BL7403', 3900000), ('BL7404', 4200000),
    ('VN7111', 3500000), ('VN7112', 3700000), ('VN7113', 3900000), ('VN7114', 4200000),
    ('VJ7211', 3500000), ('VJ7212', 3700000), ('VJ7213', 3900000), ('VJ7214', 4200000),
    ('QH7311', 3500000), ('QH7312', 3700000), ('QH7313', 3900000), ('QH7314', 4200000),
    ('BL7411', 3500000), ('BL7412', 3700000), ('BL7413', 3900000), ('BL7414', 4200000),
    -- DAD <-> SGN: 2,200,000 – 2,600,000
    ('VN7121', 2200000), ('VN7122', 2400000), ('VN7123', 2600000),
    ('VJ7221', 2200000), ('VJ7222', 2400000), ('VJ7223', 2600000),
    ('QH7321', 2200000), ('QH7322', 2400000), ('QH7323', 2600000),
    ('VN7131', 2200000), ('VN7132', 2400000), ('VN7133', 2600000),
    ('VJ7231', 2200000), ('VJ7232', 2400000), ('VJ7233', 2600000),
    ('QH7331', 2200000), ('QH7332', 2400000), ('QH7333', 2600000),
    -- HAN <-> DAD: 2,400,000 – 3,000,000
    ('VN7141', 2400000), ('VN7142', 2600000),
    ('VJ7241', 2400000), ('VJ7242', 2600000),
    ('VN7151', 2400000), ('VN7152', 2600000),
    ('VJ7251', 2400000), ('VJ7252', 2600000),
    -- SGN <-> PQC: 2,000,000 – 2,400,000
    ('VN7161', 2000000), ('VN7162', 2200000),
    ('VJ7261', 2000000), ('VJ7262', 2200000),
    ('VN7171', 2000000), ('VN7172', 2200000),
    ('VJ7271', 2000000), ('VJ7272', 2200000),
    -- SGN <-> NTR: 1,800,000 – 2,200,000
    ('VJ7281', 1800000), ('VJ7282', 2000000),
    ('QH7381', 1800000), ('QH7382', 2000000),
    ('VJ7291', 1800000), ('VJ7292', 2000000),
    ('QH7391', 1800000), ('QH7392', 2000000),
    -- HAN <-> PQC: 2,800,000 – 3,500,000
    ('VN7181', 2800000), ('VN7182', 3000000),
    ('QH7481', 2800000), ('QH7482', 3000000),
    ('VN7191', 2800000), ('VN7192', 3000000),
    ('QH7491', 2800000), ('QH7492', 3000000),
    -- SGN <-> VCA: 1,200,000 – 1,600,000
    ('VJ72A1', 1200000), ('VJ72B1', 1400000),
    -- HAN <-> HUI: 1,800,000 – 2,200,000
    ('VN71A1', 1800000), ('VN71A2', 2000000),
    ('BL74A1', 1800000), ('BL74A2', 2000000),
    ('VN71B1', 1800000), ('VN71B2', 2000000),
    ('BL74B1', 1800000), ('BL74B2', 2000000),
    -- HAN <-> HPH: 1,000,000 – 1,400,000
    ('VJ72C1', 1000000), ('VJ72D1', 1200000),
    -- SGN <-> SIN: 8,000,000
    ('SQ7701', 8000000), ('SQ7702', 8000000),
    ('SQ7711', 8000000), ('SQ7712', 8000000),
    ('VJ72E1', 8000000), ('VJ72E2', 8000000),
    ('VJ72F1', 8000000), ('VJ72F2', 8000000),
    -- HAN <-> BKK: 6,500,000
    ('TG7801', 6500000), ('TG7802', 6500000),
    ('TG7811', 6500000), ('TG7812', 6500000),
    ('VJ72G1', 6500000), ('VJ72G2', 6500000),
    ('VJ72H1', 6500000), ('VJ72H2', 6500000),
    -- SGN <-> KUL: 6,000,000
    ('VJ72I1', 6000000), ('VJ72J1', 6000000),
    -- HAN <-> TYO: 14,000,000
    ('VN71C1', 14000000), ('VN71D1', 14000000),
    -- SGN <-> SEL: 13,000,000
    ('VN71E1', 13000000), ('VN71E2', 13000000),
    ('VN71F1', 13000000), ('VN71F2', 13000000),
    ('VJ72K1', 13000000), ('VJ72K2', 13000000),
    ('VJ72L1', 13000000), ('VJ72L2', 13000000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats for Day 2
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-08';
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE CONVERT(date, f.DepartureTime) = @date
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

            // Day 3: 10/05/2026 VN = 2026-05-09 UTC
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-09';

-- Insert Flights for Day 3 (10/05 VN) - Flight numbers: 8xxx
INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, AirlineId, CreatedAt, UpdatedAt)
SELECT r.FlightNum, r.Dep, r.Arr,
       DATEADD(hour, r.DepH, CAST(@date AS DATETIME2)),
       DATEADD(hour, r.ArrH, CAST(@date AS DATETIME2)),
       r.Status, a.Id, GETUTCDATE(), GETUTCDATE()
FROM (VALUES
    -- HAN -> SGN (06:00, 08:00, 10:00, 14:00 VN = -1, 1, 3, 7 UTC, 2h flight)
    ('VN8101', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VN'),
    ('VN8102', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VN'),
    ('VN8103', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VN'),
    ('VN8104', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VN'),
    ('VJ8201', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'VJ'),
    ('VJ8202', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'VJ'),
    ('VJ8203', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'VJ'),
    ('VJ8204', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'VJ'),
    ('QH8301', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'QH'),
    ('QH8302', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'QH'),
    ('QH8303', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'QH'),
    ('QH8304', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'QH'),
    ('BL8401', 'Ha Noi', 'Ho Chi Minh', -1, 1, 'Scheduled', 'BL'),
    ('BL8402', 'Ha Noi', 'Ho Chi Minh', 1, 3, 'Scheduled', 'BL'),
    ('BL8403', 'Ha Noi', 'Ho Chi Minh', 3, 5, 'Scheduled', 'BL'),
    ('BL8404', 'Ha Noi', 'Ho Chi Minh', 7, 9, 'Scheduled', 'BL'),
    -- SGN -> HAN (07:00, 09:00, 13:00, 17:00 VN = 0, 2, 6, 10 UTC, 2h flight)
    ('VN8111', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VN'),
    ('VN8112', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VN'),
    ('VN8113', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VN'),
    ('VN8114', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VN'),
    ('VJ8211', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'VJ'),
    ('VJ8212', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'VJ'),
    ('VJ8213', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'VJ'),
    ('VJ8214', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    ('QH8311', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'QH'),
    ('QH8312', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'QH'),
    ('QH8313', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'QH'),
    ('QH8314', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'QH'),
    ('BL8411', 'Ho Chi Minh', 'Ha Noi', 0, 2, 'Scheduled', 'BL'),
    ('BL8412', 'Ho Chi Minh', 'Ha Noi', 2, 4, 'Scheduled', 'BL'),
    ('BL8413', 'Ho Chi Minh', 'Ha Noi', 6, 8, 'Scheduled', 'BL'),
    ('BL8414', 'Ho Chi Minh', 'Ha Noi', 10, 12, 'Scheduled', 'BL'),
    -- DAD -> SGN (07:00, 10:00, 15:00 VN = 0, 3, 8 UTC, 1h15 flight)
    ('VN8121', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VN'),
    ('VN8122', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VN'),
    ('VN8123', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ8221', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'VJ'),
    ('VJ8222', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ8223', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    ('QH8321', 'Da Nang', 'Ho Chi Minh', 0, 1, 'Scheduled', 'QH'),
    ('QH8322', 'Da Nang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH8323', 'Da Nang', 'Ho Chi Minh', 8, 9, 'Scheduled', 'QH'),
    -- SGN -> DAD (08:00, 12:00, 17:00 VN = 1, 5, 10 UTC, 1h15 flight)
    ('VN8131', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VN'),
    ('VN8132', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VN'),
    ('VN8133', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VN'),
    ('VJ8231', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ8232', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'VJ'),
    ('VJ8233', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'VJ'),
    ('QH8331', 'Ho Chi Minh', 'Da Nang', 1, 2, 'Scheduled', 'QH'),
    ('QH8332', 'Ho Chi Minh', 'Da Nang', 5, 6, 'Scheduled', 'QH'),
    ('QH8333', 'Ho Chi Minh', 'Da Nang', 10, 11, 'Scheduled', 'QH'),
    -- HAN -> DAD (06:30, 11:00 VN = -1, 4 UTC, 1h15 flight)
    ('VN8141', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VN'),
    ('VN8142', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VN'),
    ('VJ8241', 'Ha Noi', 'Da Nang', -1, 0, 'Scheduled', 'VJ'),
    ('VJ8242', 'Ha Noi', 'Da Nang', 4, 5, 'Scheduled', 'VJ'),
    -- DAD -> HAN (09:00, 16:00 VN = 2, 9 UTC, 1h15 flight)
    ('VN8151', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN8152', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VN'),
    ('VJ8251', 'Da Nang', 'Ha Noi', 2, 3, 'Scheduled', 'VJ'),
    ('VJ8252', 'Da Nang', 'Ha Noi', 9, 10, 'Scheduled', 'VJ'),
    -- SGN -> PQC (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN8161', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VN'),
    ('VN8162', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VN'),
    ('VJ8261', 'Ho Chi Minh', 'Phu Quoc', 0, 1, 'Scheduled', 'VJ'),
    ('VJ8262', 'Ho Chi Minh', 'Phu Quoc', 6, 7, 'Scheduled', 'VJ'),
    -- PQC -> SGN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN8171', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VN'),
    ('VN8172', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VN'),
    ('VJ8271', 'Phu Quoc', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    ('VJ8272', 'Phu Quoc', 'Ho Chi Minh', 8, 9, 'Scheduled', 'VJ'),
    -- SGN -> NTR (08:00, 14:00 VN = 1, 7 UTC, 1h flight)
    ('VJ8281', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'VJ'),
    ('VJ8282', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'VJ'),
    ('QH8381', 'Ho Chi Minh', 'Nha Trang', 1, 2, 'Scheduled', 'QH'),
    ('QH8382', 'Ho Chi Minh', 'Nha Trang', 7, 8, 'Scheduled', 'QH'),
    -- NTR -> SGN (10:00, 16:00 VN = 3, 9 UTC, 1h flight)
    ('VJ8291', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'VJ'),
    ('VJ8292', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'VJ'),
    ('QH8391', 'Nha Trang', 'Ho Chi Minh', 3, 4, 'Scheduled', 'QH'),
    ('QH8392', 'Nha Trang', 'Ho Chi Minh', 9, 10, 'Scheduled', 'QH'),
    -- HAN -> PQC (07:00, 12:00 VN = 0, 5 UTC, 2h15 flight)
    ('VN8181', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'VN'),
    ('VN8182', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'VN'),
    ('QH8481', 'Ha Noi', 'Phu Quoc', 0, 2, 'Scheduled', 'QH'),
    ('QH8482', 'Ha Noi', 'Phu Quoc', 5, 7, 'Scheduled', 'QH'),
    -- PQC -> HAN (10:00, 15:00 VN = 3, 8 UTC, 2h15 flight)
    ('VN8191', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'VN'),
    ('VN8192', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'VN'),
    ('QH8491', 'Phu Quoc', 'Ha Noi', 3, 5, 'Scheduled', 'QH'),
    ('QH8492', 'Phu Quoc', 'Ha Noi', 8, 10, 'Scheduled', 'QH'),
    -- SGN -> VCA (08:00 VN = 1 UTC, 0h45 flight)
    ('VJ82A1', 'Ho Chi Minh', 'Can Tho', 1, 2, 'Scheduled', 'VJ'),
    -- VCA -> SGN (09:30 VN = 2 UTC approx, 0h45 flight)
    ('VJ82B1', 'Can Tho', 'Ho Chi Minh', 2, 3, 'Scheduled', 'VJ'),
    -- HAN -> HUI (07:00, 13:00 VN = 0, 6 UTC, 1h flight)
    ('VN81A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'VN'),
    ('VN81A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'VN'),
    ('BL84A1', 'Ha Noi', 'Hue', 0, 1, 'Scheduled', 'BL'),
    ('BL84A2', 'Ha Noi', 'Hue', 6, 7, 'Scheduled', 'BL'),
    -- HUI -> HAN (09:00, 15:00 VN = 2, 8 UTC, 1h flight)
    ('VN81B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'VN'),
    ('VN81B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'VN'),
    ('BL84B1', 'Hue', 'Ha Noi', 2, 3, 'Scheduled', 'BL'),
    ('BL84B2', 'Hue', 'Ha Noi', 8, 9, 'Scheduled', 'BL'),
    -- HAN -> HPH (08:00 VN = 1 UTC, 0h30 flight)
    ('VJ82C1', 'Ha Noi', 'Hai Phong', 1, 1, 'Scheduled', 'VJ'),
    -- HPH -> HAN (09:00 VN = 2 UTC, 0h30 flight)
    ('VJ82D1', 'Hai Phong', 'Ha Noi', 2, 2, 'Scheduled', 'VJ'),
    -- SGN -> SIN (08:00, 11:00 VN = 1, 4 UTC, 2h flight)
    ('SQ8701', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'SQ'),
    ('SQ8702', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'SQ'),
    ('VJ82E1', 'Ho Chi Minh', 'Singapore', 1, 3, 'Scheduled', 'VJ'),
    ('VJ82E2', 'Ho Chi Minh', 'Singapore', 4, 6, 'Scheduled', 'VJ'),
    -- SIN -> SGN (13:00, 16:00 VN = 6, 9 UTC, 2h flight)
    ('SQ8711', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'SQ'),
    ('SQ8712', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'SQ'),
    ('VJ82F1', 'Singapore', 'Ho Chi Minh', 6, 8, 'Scheduled', 'VJ'),
    ('VJ82F2', 'Singapore', 'Ho Chi Minh', 9, 11, 'Scheduled', 'VJ'),
    -- HAN -> BKK (09:00, 14:00 VN = 2, 7 UTC, 2h flight)
    ('TG8801', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'TG'),
    ('TG8802', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'TG'),
    ('VJ82G1', 'Ha Noi', 'Bangkok', 2, 4, 'Scheduled', 'VJ'),
    ('VJ82G2', 'Ha Noi', 'Bangkok', 7, 9, 'Scheduled', 'VJ'),
    -- BKK -> HAN (12:00, 17:00 VN = 5, 10 UTC, 2h flight)
    ('TG8811', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'TG'),
    ('TG8812', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'TG'),
    ('VJ82H1', 'Bangkok', 'Ha Noi', 5, 7, 'Scheduled', 'VJ'),
    ('VJ82H2', 'Bangkok', 'Ha Noi', 10, 12, 'Scheduled', 'VJ'),
    -- SGN -> KUL (09:00 VN = 2 UTC, 2h flight)
    ('VJ82I1', 'Ho Chi Minh', 'Kuala Lumpur', 2, 4, 'Scheduled', 'VJ'),
    -- KUL -> SGN (12:30 VN = 2 UTC approx, 2h flight)
    ('VJ82J1', 'Kuala Lumpur', 'Ho Chi Minh', 2, 4, 'Scheduled', 'VJ'),
    -- HAN -> TYO (10:00 VN = 3 UTC, 5h flight)
    ('VN81C1', 'Ha Noi', 'Tokyo', 3, 8, 'Scheduled', 'VN'),
    -- TYO -> HAN (16:00 VN = 9 UTC, 5h flight)
    ('VN81D1', 'Tokyo', 'Ha Noi', 9, 14, 'Scheduled', 'VN'),
    -- SGN -> SEL (08:00, 22:00 VN = 1, 15 UTC, 5h flight)
    ('VN81E1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VN'),
    ('VN81E2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VN'),
    ('VJ82K1', 'Ho Chi Minh', 'Seoul', 1, 6, 'Scheduled', 'VJ'),
    ('VJ82K2', 'Ho Chi Minh', 'Seoul', 15, 20, 'Scheduled', 'VJ'),
    -- SEL -> SGN (14:00, 06:00+1 VN = 7, -1 UTC, 5h flight)
    ('VN81F1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VN'),
    ('VN81F2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VN'),
    ('VJ82L1', 'Seoul', 'Ho Chi Minh', 7, 12, 'Scheduled', 'VJ'),
    ('VJ82L2', 'Seoul', 'Ho Chi Minh', -1, 4, 'Scheduled', 'VJ')
) r(FlightNum, Dep, Arr, DepH, ArrH, Status, AirlineCode)
JOIN Airlines a ON a.Code = r.AirlineCode
WHERE NOT EXISTS (SELECT 1 FROM Flights f WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date);
");

            // Insert FlightPrices for Day 3
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-09';

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Economy', r.EconPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 1,500,000 – 1,800,000
    ('VN8101', 1500000), ('VN8102', 1600000), ('VN8103', 1700000), ('VN8104', 1800000),
    ('VJ8201', 1500000), ('VJ8202', 1600000), ('VJ8203', 1700000), ('VJ8204', 1800000),
    ('QH8301', 1500000), ('QH8302', 1600000), ('QH8303', 1700000), ('QH8304', 1800000),
    ('BL8401', 1500000), ('BL8402', 1600000), ('BL8403', 1700000), ('BL8404', 1800000),
    ('VN8111', 1500000), ('VN8112', 1600000), ('VN8113', 1700000), ('VN8114', 1800000),
    ('VJ8211', 1500000), ('VJ8212', 1600000), ('VJ8213', 1700000), ('VJ8214', 1800000),
    ('QH8311', 1500000), ('QH8312', 1600000), ('QH8313', 1700000), ('QH8314', 1800000),
    ('BL8411', 1500000), ('BL8412', 1600000), ('BL8413', 1700000), ('BL8414', 1800000),
    -- DAD <-> SGN: 900,000 – 1,100,000
    ('VN8121', 900000), ('VN8122', 1000000), ('VN8123', 1100000),
    ('VJ8221', 900000), ('VJ8222', 1000000), ('VJ8223', 1100000),
    ('QH8321', 900000), ('QH8322', 1000000), ('QH8323', 1100000),
    ('VN8131', 900000), ('VN8132', 1000000), ('VN8133', 1100000),
    ('VJ8231', 900000), ('VJ8232', 1000000), ('VJ8233', 1100000),
    ('QH8331', 900000), ('QH8332', 1000000), ('QH8333', 1100000),
    -- HAN <-> DAD: 1,000,000 – 1,300,000
    ('VN8141', 1000000), ('VN8142', 1100000),
    ('VJ8241', 1000000), ('VJ8242', 1100000),
    ('VN8151', 1000000), ('VN8152', 1100000),
    ('VJ8251', 1000000), ('VJ8252', 1100000),
    -- SGN <-> PQC: 800,000 – 1,000,000
    ('VN8161', 800000), ('VN8162', 900000),
    ('VJ8261', 800000), ('VJ8262', 900000),
    ('VN8171', 800000), ('VN8172', 900000),
    ('VJ8271', 800000), ('VJ8272', 900000),
    -- SGN <-> NTR: 700,000 – 900,000
    ('VJ8281', 700000), ('VJ8282', 800000),
    ('QH8381', 700000), ('QH8382', 800000),
    ('VJ8291', 700000), ('VJ8292', 800000),
    ('QH8391', 700000), ('QH8392', 800000),
    -- HAN <-> PQC: 1,200,000 – 1,500,000
    ('VN8181', 1200000), ('VN8182', 1300000),
    ('QH8481', 1200000), ('QH8482', 1300000),
    ('VN8191', 1200000), ('VN8192', 1300000),
    ('QH8491', 1200000), ('QH8492', 1300000),
    -- SGN <-> VCA: 500,000 – 700,000
    ('VJ82A1', 500000), ('VJ82B1', 600000),
    -- HAN <-> HUI: 700,000 – 900,000
    ('VN81A1', 700000), ('VN81A2', 800000),
    ('BL84A1', 700000), ('BL84A2', 800000),
    ('VN81B1', 700000), ('VN81B2', 800000),
    ('BL84B1', 700000), ('BL84B2', 800000),
    -- HAN <-> HPH: 400,000 – 600,000
    ('VJ82C1', 400000), ('VJ82D1', 500000),
    -- SGN <-> SIN: 3,500,000
    ('SQ8701', 3500000), ('SQ8702', 3500000),
    ('SQ8711', 3500000), ('SQ8712', 3500000),
    ('VJ82E1', 3500000), ('VJ82E2', 3500000),
    ('VJ82F1', 3500000), ('VJ82F2', 3500000),
    -- HAN <-> BKK: 2,800,000
    ('TG8801', 2800000), ('TG8802', 2800000),
    ('TG8811', 2800000), ('TG8812', 2800000),
    ('VJ82G1', 2800000), ('VJ82G2', 2800000),
    ('VJ82H1', 2800000), ('VJ82H2', 2800000),
    -- SGN <-> KUL: 2,500,000
    ('VJ82I1', 2500000), ('VJ82J1', 2500000),
    -- HAN <-> TYO: 6,000,000
    ('VN81C1', 6000000), ('VN81D1', 6000000),
    -- SGN <-> SEL: 5,500,000
    ('VN81E1', 5500000), ('VN81E2', 5500000),
    ('VN81F1', 5500000), ('VN81F2', 5500000),
    ('VJ82K1', 5500000), ('VJ82K2', 5500000),
    ('VJ82L1', 5500000), ('VJ82L2', 5500000)
) r(FlightNum, EconPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
SELECT f.Id, 'Business', r.BizPrice, GETUTCDATE(), GETUTCDATE()
FROM Flights f
CROSS APPLY (VALUES
    -- HAN <-> SGN: 3,500,000 – 4,200,000
    ('VN8101', 3500000), ('VN8102', 3700000), ('VN8103', 3900000), ('VN8104', 4200000),
    ('VJ8201', 3500000), ('VJ8202', 3700000), ('VJ8203', 3900000), ('VJ8204', 4200000),
    ('QH8301', 3500000), ('QH8302', 3700000), ('QH8303', 3900000), ('QH8304', 4200000),
    ('BL8401', 3500000), ('BL8402', 3700000), ('BL8403', 3900000), ('BL8404', 4200000),
    ('VN8111', 3500000), ('VN8112', 3700000), ('VN8113', 3900000), ('VN8114', 4200000),
    ('VJ8211', 3500000), ('VJ8212', 3700000), ('VJ8213', 3900000), ('VJ8214', 4200000),
    ('QH8311', 3500000), ('QH8312', 3700000), ('QH8313', 3900000), ('QH8314', 4200000),
    ('BL8411', 3500000), ('BL8412', 3700000), ('BL8413', 3900000), ('BL8414', 4200000),
    -- DAD <-> SGN: 2,200,000 – 2,600,000
    ('VN8121', 2200000), ('VN8122', 2400000), ('VN8123', 2600000),
    ('VJ8221', 2200000), ('VJ8222', 2400000), ('VJ8223', 2600000),
    ('QH8321', 2200000), ('QH8322', 2400000), ('QH8323', 2600000),
    ('VN8131', 2200000), ('VN8132', 2400000), ('VN8133', 2600000),
    ('VJ8231', 2200000), ('VJ8232', 2400000), ('VJ8233', 2600000),
    ('QH8331', 2200000), ('QH8332', 2400000), ('QH8333', 2600000),
    -- HAN <-> DAD: 2,400,000 – 3,000,000
    ('VN8141', 2400000), ('VN8142', 2600000),
    ('VJ8241', 2400000), ('VJ8242', 2600000),
    ('VN8151', 2400000), ('VN8152', 2600000),
    ('VJ8251', 2400000), ('VJ8252', 2600000),
    -- SGN <-> PQC: 2,000,000 – 2,400,000
    ('VN8161', 2000000), ('VN8162', 2200000),
    ('VJ8261', 2000000), ('VJ8262', 2200000),
    ('VN8171', 2000000), ('VN8172', 2200000),
    ('VJ8271', 2000000), ('VJ8272', 2200000),
    -- SGN <-> NTR: 1,800,000 – 2,200,000
    ('VJ8281', 1800000), ('VJ8282', 2000000),
    ('QH8381', 1800000), ('QH8382', 2000000),
    ('VJ8291', 1800000), ('VJ8292', 2000000),
    ('QH8391', 1800000), ('QH8392', 2000000),
    -- HAN <-> PQC: 2,800,000 – 3,500,000
    ('VN8181', 2800000), ('VN8182', 3000000),
    ('QH8481', 2800000), ('QH8482', 3000000),
    ('VN8191', 2800000), ('VN8192', 3000000),
    ('QH8491', 2800000), ('QH8492', 3000000),
    -- SGN <-> VCA: 1,200,000 – 1,600,000
    ('VJ82A1', 1200000), ('VJ82B1', 1400000),
    -- HAN <-> HUI: 1,800,000 – 2,200,000
    ('VN81A1', 1800000), ('VN81A2', 2000000),
    ('BL84A1', 1800000), ('BL84A2', 2000000),
    ('VN81B1', 1800000), ('VN81B2', 2000000),
    ('BL84B1', 1800000), ('BL84B2', 2000000),
    -- HAN <-> HPH: 1,000,000 – 1,400,000
    ('VJ82C1', 1000000), ('VJ82D1', 1200000),
    -- SGN <-> SIN: 8,000,000
    ('SQ8701', 8000000), ('SQ8702', 8000000),
    ('SQ8711', 8000000), ('SQ8712', 8000000),
    ('VJ82E1', 8000000), ('VJ82E2', 8000000),
    ('VJ82F1', 8000000), ('VJ82F2', 8000000),
    -- HAN <-> BKK: 6,500,000
    ('TG8801', 6500000), ('TG8802', 6500000),
    ('TG8811', 6500000), ('TG8812', 6500000),
    ('VJ82G1', 6500000), ('VJ82G2', 6500000),
    ('VJ82H1', 6500000), ('VJ82H2', 6500000),
    -- SGN <-> KUL: 6,000,000
    ('VJ82I1', 6000000), ('VJ82J1', 6000000),
    -- HAN <-> TYO: 14,000,000
    ('VN81C1', 14000000), ('VN81D1', 14000000),
    -- SGN <-> SEL: 13,000,000
    ('VN81E1', 13000000), ('VN81E2', 13000000),
    ('VN81F1', 13000000), ('VN81F2', 13000000),
    ('VJ82K1', 13000000), ('VJ82K2', 13000000),
    ('VJ82L1', 13000000), ('VJ82L2', 13000000)
) r(FlightNum, BizPrice)
WHERE f.FlightNumber = r.FlightNum AND CONVERT(date, f.DepartureTime) = @date
AND NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');
");

            // Insert Seats for Day 3
            migrationBuilder.Sql(@"
DECLARE @date DATE = '2026-05-09';
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR 
    SELECT f.Id FROM Flights f
    WHERE CONVERT(date, f.DepartureTime) = @date
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
            migrationBuilder.Sql(@"
-- Delete Seats for seeded flights
DELETE FROM Seats WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
    AND (
        FlightNumber LIKE '6[1-4]%' OR FlightNumber LIKE '7[1-4]%' OR FlightNumber LIKE '8[1-4]%'
        OR FlightNumber LIKE 'VJ6%' OR FlightNumber LIKE 'VJ7%' OR FlightNumber LIKE 'VJ8%'
        OR FlightNumber LIKE 'QH6%' OR FlightNumber LIKE 'QH7%' OR FlightNumber LIKE 'QH8%'
        OR FlightNumber LIKE 'BL6%' OR FlightNumber LIKE 'BL7%' OR FlightNumber LIKE 'BL8%'
        OR FlightNumber IN ('SQ6701','SQ6702','SQ6711','SQ6712','TG6801','TG6802','TG6811','TG6812',
                            'SQ7701','SQ7702','SQ7711','SQ7712','TG7801','TG7802','TG7811','TG7812',
                            'SQ8701','SQ8702','SQ8711','SQ8712','TG8801','TG8802','TG8811','TG8812')
    )
);

-- Delete FlightPrices for seeded flights
DELETE FROM FlightPrices WHERE FlightId IN (
    SELECT Id FROM Flights
    WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
    AND (
        FlightNumber LIKE '6[1-4]%' OR FlightNumber LIKE '7[1-4]%' OR FlightNumber LIKE '8[1-4]%'
        OR FlightNumber LIKE 'VJ6%' OR FlightNumber LIKE 'VJ7%' OR FlightNumber LIKE 'VJ8%'
        OR FlightNumber LIKE 'QH6%' OR FlightNumber LIKE 'QH7%' OR FlightNumber LIKE 'QH8%'
        OR FlightNumber LIKE 'BL6%' OR FlightNumber LIKE 'BL7%' OR FlightNumber LIKE 'BL8%'
        OR FlightNumber IN ('SQ6701','SQ6702','SQ6711','SQ6712','TG6801','TG6802','TG6811','TG6812',
                            'SQ7701','SQ7702','SQ7711','SQ7712','TG7801','TG7802','TG7811','TG7812',
                            'SQ8701','SQ8702','SQ8711','SQ8712','TG8801','TG8802','TG8811','TG8812')
    )
);

-- Delete seeded flights
DELETE FROM Flights
WHERE CONVERT(date, DepartureTime) IN ('2026-05-07','2026-05-08','2026-05-09')
AND (
    FlightNumber LIKE '6[1-4]%' OR FlightNumber LIKE '7[1-4]%' OR FlightNumber LIKE '8[1-4]%'
    OR FlightNumber LIKE 'VJ6%' OR FlightNumber LIKE 'VJ7%' OR FlightNumber LIKE 'VJ8%'
    OR FlightNumber LIKE 'QH6%' OR FlightNumber LIKE 'QH7%' OR FlightNumber LIKE 'QH8%'
    OR FlightNumber LIKE 'BL6%' OR FlightNumber LIKE 'BL7%' OR FlightNumber LIKE 'BL8%'
    OR FlightNumber IN ('SQ6701','SQ6702','SQ6711','SQ6712','TG6801','TG6802','TG6811','TG6812',
                        'SQ7701','SQ7702','SQ7711','SQ7712','TG7801','TG7802','TG7811','TG7812',
                        'SQ8701','SQ8702','SQ8711','SQ8712','TG8801','TG8802','TG8811','TG8812')
);
");
        }
    }
}
