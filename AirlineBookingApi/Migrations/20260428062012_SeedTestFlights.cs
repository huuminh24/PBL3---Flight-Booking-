using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed test flights for the next 7 days
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN101' AND DepartureTime = '2026-05-01 08:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN101', 'Da Nang', 'Ho Chi Minh', '2026-05-01 08:00:00', '2026-05-01 09:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());

                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN102' AND DepartureTime = '2026-05-01 11:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN102', 'Ho Chi Minh', 'Da Nang', '2026-05-01 11:00:00', '2026-05-01 12:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());

                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN201' AND DepartureTime = '2026-05-01 14:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN201', 'Ha Noi', 'Da Nang', '2026-05-01 14:00:00', '2026-05-01 15:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());

                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN202' AND DepartureTime = '2026-05-01 17:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN202', 'Da Nang', 'Ha Noi', '2026-05-01 17:00:00', '2026-05-01 18:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());

                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN301' AND DepartureTime = '2026-05-01 20:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN301', 'Ha Noi', 'Ho Chi Minh', '2026-05-01 20:00:00', '2026-05-01 21:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());

                IF NOT EXISTS (SELECT 1 FROM Flights WHERE FlightNumber = 'VN302' AND DepartureTime = '2026-05-01 23:00:00')
                    INSERT INTO Flights (FlightNumber, DepartureAirport, ArrivalAirport, DepartureTime, ArrivalTime, Status, CreatedAt, UpdatedAt)
                    VALUES ('VN302', 'Ho Chi Minh', 'Ha Noi', '2026-05-01 23:00:00', '2026-05-02 00:00:00', 'Scheduled', GETUTCDATE(), GETUTCDATE());
            ");

            // Seed flight prices
            migrationBuilder.Sql(@"
                INSERT INTO FlightPrices (FlightId, SeatClass, Price, CreatedAt, UpdatedAt)
                SELECT f.Id, 'Economy', 1500000, GETUTCDATE(), GETUTCDATE()
                FROM Flights f
                WHERE NOT EXISTS (
                    SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Economy'
                )
                UNION ALL
                SELECT f.Id, 'Business', 3500000, GETUTCDATE(), GETUTCDATE()
                FROM Flights f
                WHERE NOT EXISTS (
                    SELECT 1 FROM FlightPrices fp WHERE fp.FlightId = f.Id AND fp.SeatClass = 'Business'
                )
            ");

            // Seed seats for each flight (30 seats per flight: 20 Economy, 10 Business)
            migrationBuilder.Sql(@"
                DECLARE @flightId INT;
                DECLARE flight_cursor CURSOR FOR SELECT Id FROM Flights;
                OPEN flight_cursor;
                FETCH NEXT FROM flight_cursor INTO @flightId;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    -- Economy seats (A1-A10, B1-B10)
                    DECLARE @i INT = 1;
                    WHILE @i <= 10
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = 'A' + CAST(@i AS VARCHAR))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, 'A' + CAST(@i AS VARCHAR), 'Economy', 1, GETUTCDATE(), GETUTCDATE());

                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = 'B' + CAST(@i AS VARCHAR))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, 'B' + CAST(@i AS VARCHAR), 'Economy', 1, GETUTCDATE(), GETUTCDATE());
                        SET @i = @i + 1;
                    END;
                    
                    -- Business seats (C1-C5, D1-D5)
                    SET @i = 1;
                    WHILE @i <= 5
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = 'C' + CAST(@i AS VARCHAR))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, 'C' + CAST(@i AS VARCHAR), 'Business', 1, GETUTCDATE(), GETUTCDATE());

                        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = 'D' + CAST(@i AS VARCHAR))
                            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
                            VALUES (@flightId, 'D' + CAST(@i AS VARCHAR), 'Business', 1, GETUTCDATE(), GETUTCDATE());
                        SET @i = @i + 1;
                    END;
                    
                    FETCH NEXT FROM flight_cursor INTO @flightId;
                END;
                CLOSE flight_cursor;
                DEALLOCATE flight_cursor;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
