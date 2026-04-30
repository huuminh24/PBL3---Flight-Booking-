using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    [Migration("20260430093000_SeedAgencyDemoFlightsAndOversell")]
    public partial class SeedAgencyDemoFlightsAndOversell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @demoDate DATE = CONVERT(date, DATEADD(day, 7, GETUTCDATE()));

                DECLARE @DemoFlights TABLE(
                    FlightNumber NVARCHAR(20), Dep NVARCHAR(100), Arr NVARCHAR(100), DepHour INT, ArrHour INT, Status NVARCHAR(30), EconomyPrice DECIMAL(18,2), BusinessPrice DECIMAL(18,2)
                );

                INSERT INTO @DemoFlights VALUES
                ('VN901', 'Ha Noi', 'Ho Chi Minh', 6, 8, 'Scheduled', 1600000, 3600000),
                ('VJ321', 'Ha Noi', 'Da Nang', 8, 9, 'Boarding', 900000, 2200000),
                ('QH205', 'Da Nang', 'Ho Chi Minh', 10, 12, 'Delayed', 1200000, 2800000),
                ('BL442', 'Ho Chi Minh', 'Da Nang', 13, 14, 'Scheduled', 950000, 2300000),
                ('SQ177', 'Ho Chi Minh', 'Singapore', 15, 18, 'Scheduled', 2800000, 6500000),
                ('TG551', 'Ha Noi', 'Bangkok', 17, 20, 'Scheduled', 2400000, 5900000),
                ('VJ888', 'Da Nang', 'Ha Noi', 19, 20, 'Arrived', 1000000, 2500000),
                ('VN777', 'Ho Chi Minh', 'Phu Quoc', 21, 22, 'Cancelled', 850000, 2100000);

                INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                SELECT d.FlightNumber, d.Dep, d.Arr,
                       DATEADD(hour, d.DepHour, CAST(@demoDate AS DATETIME2)),
                       DATEADD(hour, d.ArrHour, CAST(@demoDate AS DATETIME2)),
                       d.Status, GETUTCDATE(), GETUTCDATE()
                FROM @DemoFlights d
                WHERE NOT EXISTS (
                    SELECT 1 FROM Flights f
                    WHERE f.FlightNumber = d.FlightNumber
                      AND CONVERT(date, f.DepartureTime) = @demoDate
                );

                INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
                SELECT f.Id, 'Economy', d.EconomyPrice, GETUTCDATE(), GETUTCDATE()
                FROM Flights f
                JOIN @DemoFlights d ON d.FlightNumber = f.FlightNumber AND CONVERT(date, f.DepartureTime) = @demoDate
                WHERE NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy');

                INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
                SELECT f.Id, 'Business', d.BusinessPrice, GETUTCDATE(), GETUTCDATE()
                FROM Flights f
                JOIN @DemoFlights d ON d.FlightNumber = f.FlightNumber AND CONVERT(date, f.DepartureTime) = @demoDate
                WHERE NOT EXISTS (SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business');

                DECLARE @flightId INT;
                DECLARE c CURSOR FOR
                    SELECT f.Id FROM Flights f
                    JOIN @DemoFlights d ON d.FlightNumber = f.FlightNumber AND CONVERT(date, f.DepartureTime) = @demoDate;
                OPEN c;
                FETCH NEXT FROM c INTO @flightId;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DECLARE @i INT = 1;
                    WHILE @i <= 8
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = CONCAT('E', @i))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, CONCAT('E', @i), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
                        SET @i += 1;
                    END;

                    SET @i = 1;
                    WHILE @i <= 4
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = CONCAT('B', @i))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, CONCAT('B', @i), 'Business', 1, GETUTCDATE(), GETUTCDATE());
                        SET @i += 1;
                    END;
                    FETCH NEXT FROM c INTO @flightId;
                END;
                CLOSE c;
                DEALLOCATE c;

                DECLARE @oversellFlightId INT = (
                    SELECT TOP 1 Id FROM Flights
                    WHERE FlightNumber = 'VJ321' AND CONVERT(date, DepartureTime) = @demoDate
                    ORDER BY Id DESC
                );

                IF @oversellFlightId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Bookings WHERE PnrCode = 'OSDEMO1')
                BEGIN
                    DECLARE @createdByAccountId INT = ISNULL((SELECT TOP 1 Id FROM Accounts ORDER BY Id), 1);
                    DECLARE @bfId INT;
                    DECLARE @bookingId INT;
                    DECLARE @passengerId INT;
                    DECLARE @ticketId INT;
                    DECLARE @n INT = 1;
                    DECLARE @seatId INT;
                    DECLARE @amount DECIMAL(18,2) = 900000;

                    WHILE @n <= 10
                    BEGIN
                        INSERT INTO Bookings (PnrCode, BookingStatus, BookingChannel, CustomerAccountId, CreatedByAccountId, ContactFullName, ContactEmail, ContactPhoneNumber, TotalAmount, ExpiresAt, CreatedAt, UpdatedAt)
                        VALUES (CONCAT('OSDEMO', @n), 'Paid', CASE WHEN @n % 2 = 0 THEN 'Staff' ELSE 'Website' END, NULL, @createdByAccountId,
                                CONCAT(N'Khách Vé Tồn ', @n), CONCAT('oversell', @n, '@demo.local'), CONCAT('09000000', @n), @amount, DATEADD(day, 1, GETUTCDATE()), GETUTCDATE(), GETUTCDATE());
                        SET @bookingId = SCOPE_IDENTITY();

                        INSERT INTO BookingFlights (BookingId, FlightId, LegOrder, SeatClass, Price, CreatedAt, UpdatedAt)
                        VALUES (@bookingId, @oversellFlightId, 1, 'Economy', @amount, GETUTCDATE(), GETUTCDATE());
                        SET @bfId = SCOPE_IDENTITY();

                        INSERT INTO Passengers (BookingId, FullName, Gender, PassengerType, CreatedAt, UpdatedAt)
                        VALUES (@bookingId, CONCAT(N'Khách Vé Tồn ', @n), CASE WHEN @n % 2 = 0 THEN 'Female' ELSE 'Male' END, 'Adult', GETUTCDATE(), GETUTCDATE());
                        SET @passengerId = SCOPE_IDENTITY();

                        SELECT @seatId = Id FROM Seats WHERE FlightId = @oversellFlightId AND SeatClass = 'Economy' AND SeatNumber = CONCAT('E', @n);

                        INSERT INTO Tickets (BookingId, PassengerId, BookingFlightId, FlightId, SeatId, SeatClass, Price, TicketStatus, AllowCancellation, IsCheckedIn, CreatedAt, UpdatedAt)
                        VALUES (@bookingId, @passengerId, @bfId, @oversellFlightId, CASE WHEN @n <= 8 THEN @seatId ELSE NULL END, 'Economy', @amount, 'Paid', 1, CASE WHEN @n <= 6 THEN 1 ELSE 0 END, GETUTCDATE(), GETUTCDATE());
                        SET @ticketId = SCOPE_IDENTITY();

                        INSERT INTO Payments (BookingId, PaymentMethod, Amount, DiscountAmount, CouponCode, PaymentStatus, PaidAt, PaymentReference, CreatedAt, UpdatedAt)
                        VALUES (@bookingId, CASE WHEN @n % 3 = 0 THEN 'VietQR' WHEN @n % 3 = 1 THEN 'Card' ELSE 'BankTransfer' END, @amount, 0, NULL, 'Paid', DATEADD(day, -(@n % 6), GETUTCDATE()), CONCAT('DEMO-', @ticketId), GETUTCDATE(), GETUTCDATE());

                        SET @n += 1;
                    END;
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
