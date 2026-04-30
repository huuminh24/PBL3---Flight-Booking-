using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    public partial class TopUpSeatsForAllFlights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @existingEcon INT;
DECLARE @existingBiz INT;
DECLARE @i INT;
DECLARE @seatNumber NVARCHAR(10);

DECLARE flightCursor CURSOR FAST_FORWARD FOR SELECT Id FROM Flights;
OPEN flightCursor;
FETCH NEXT FROM flightCursor INTO @flightId;
WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT @existingEcon = COUNT(*) FROM Seats WHERE FlightId = @flightId AND SeatClass = 'Economy';
    SELECT @existingBiz  = COUNT(*) FROM Seats WHERE FlightId = @flightId AND SeatClass = 'Business';

    SET @i = 1;
    WHILE @existingEcon < 30 AND @i <= 100
    BEGIN
        SET @seatNumber = CONCAT('TE', RIGHT('00' + CAST(@i AS NVARCHAR(3)), 3));
        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = @seatNumber)
        BEGIN
            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
            VALUES (@flightId, @seatNumber, 'Economy', 1, GETUTCDATE(), GETUTCDATE());
            SET @existingEcon = @existingEcon + 1;
        END
        SET @i = @i + 1;
    END;

    SET @i = 1;
    WHILE @existingBiz < 12 AND @i <= 100
    BEGIN
        SET @seatNumber = CONCAT('TB', RIGHT('00' + CAST(@i AS NVARCHAR(3)), 3));
        IF NOT EXISTS (SELECT 1 FROM Seats WHERE FlightId = @flightId AND SeatNumber = @seatNumber)
        BEGIN
            INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
            VALUES (@flightId, @seatNumber, 'Business', 1, GETUTCDATE(), GETUTCDATE());
            SET @existingBiz = @existingBiz + 1;
        END
        SET @i = @i + 1;
    END;

    FETCH NEXT FROM flightCursor INTO @flightId;
END;
CLOSE flightCursor;
DEALLOCATE flightCursor;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}

