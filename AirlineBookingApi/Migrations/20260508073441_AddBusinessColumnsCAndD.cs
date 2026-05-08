using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessColumnsCAndD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Business class seats for columns C and D (B1C -> B6C, B1D -> B6D)
            // This expands Business from 2 columns (A-B) to 4 columns (A-B-C-D)
            // Result: 6 rows x 4 columns = 24 Business seats per flight
            migrationBuilder.Sql(@"
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR SELECT Id FROM Flights;
OPEN flightCur;
FETCH NEXT FROM flightCur INTO @flightId;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Add Business columns C and D for rows 1-6
    SET @row = 1;
    WHILE @row <= 6
    BEGIN
        -- Column C (3rd column)
        SET @col = 3;
        SET @sn = N'B' + CAST(@row AS NVARCHAR(2)) + CHAR(64 + @col);
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@flightId, @sn, N'Business', 1, GETUTCDATE(), GETUTCDATE());

        -- Column D (4th column)
        SET @col = 4;
        SET @sn = N'B' + CAST(@row AS NVARCHAR(2)) + CHAR(64 + @col);
        INSERT INTO Seats (FlightId, SeatNumber, SeatClass, IsAvailable, CreatedAt, UpdatedAt)
        VALUES (@flightId, @sn, N'Business', 1, GETUTCDATE(), GETUTCDATE());

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
            // Remove the added Business seats for columns C and D (B1C -> B6C, B1D -> B6D)
            migrationBuilder.Sql(@"
DELETE FROM Seats WHERE SeatClass = N'Business' AND (
    SeatNumber LIKE N'%C' OR SeatNumber LIKE N'%D'
);
");
        }
    }
}
