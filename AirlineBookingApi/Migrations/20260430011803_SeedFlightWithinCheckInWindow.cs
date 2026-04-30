using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedFlightWithinCheckInWindow : Migration
    {
        // Seeds 1 demo flight (VN9001) at 2026-05-01 12:00 UTC plus a Paid booking for the
        // demo customer (customer@airline.com) so testers can exercise the check-in window.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Flight + prices + seats.
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Flights] WHERE [FlightNumber] = N'VN9001')
BEGIN
    INSERT INTO [Flights] ([ArrivalAirport], [ArrivalTime], [CreatedAt], [DepartureAirport], [DepartureTime], [FlightNumber], [Status], [UpdatedAt])
    VALUES (N'Ho Chi Minh', '2026-05-01T13:30:00', '2026-01-01T00:00:00', N'Da Nang', '2026-05-01T12:00:00', N'VN9001', N'Scheduled', NULL);
END
");

            migrationBuilder.Sql(@"
INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 1250000, N'Economy', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN9001'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Economy');

INSERT INTO [FlightPrices] ([CreatedAt], [FlightId], [Price], [SeatClass], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 2300000, N'Business', NULL
FROM [Flights] f
WHERE f.[FlightNumber] = N'VN9001'
  AND NOT EXISTS (SELECT 1 FROM [FlightPrices] fp WHERE fp.[FlightId] = f.[Id] AND fp.[SeatClass] = N'Business');
");

            migrationBuilder.Sql(@"
;WITH SeatCatalog(SeatNumber, SeatClass) AS (
    SELECT N'E01', N'Economy' UNION ALL
    SELECT N'E02', N'Economy' UNION ALL
    SELECT N'E03', N'Economy' UNION ALL
    SELECT N'B01', N'Business' UNION ALL
    SELECT N'B02', N'Business'
)
INSERT INTO [Seats] ([CreatedAt], [FlightId], [IsAvailable], [SeatClass], [SeatNumber], [UpdatedAt])
SELECT '2026-01-01T00:00:00', f.[Id], 1, sc.SeatClass, sc.SeatNumber, NULL
FROM [Flights] f
CROSS JOIN SeatCatalog sc
WHERE f.[FlightNumber] = N'VN9001'
  AND NOT EXISTS (SELECT 1 FROM [Seats] s WHERE s.[FlightId] = f.[Id] AND s.[SeatNumber] = sc.SeatNumber);
");

            // 2. Paid booking for the demo customer (customer@airline.com), one Adult on E01.
            migrationBuilder.Sql(@"
DECLARE @CustomerId INT = (SELECT TOP 1 [Id] FROM [Accounts] WHERE [Email] = N'customer@airline.com');
DECLARE @StaffId    INT = (SELECT TOP 1 [Id] FROM [Accounts] WHERE [Email] = N'staff@airline.com');
DECLARE @FlightId   INT = (SELECT TOP 1 [Id] FROM [Flights]  WHERE [FlightNumber] = N'VN9001');
DECLARE @SeatId     INT = (SELECT TOP 1 [Id] FROM [Seats]    WHERE [FlightId] = @FlightId AND [SeatNumber] = N'E01');
DECLARE @PriceEcon  DECIMAL(18,2) = (SELECT TOP 1 [Price] FROM [FlightPrices] WHERE [FlightId] = @FlightId AND [SeatClass] = N'Economy');

IF @CustomerId IS NULL OR @StaffId IS NULL OR @FlightId IS NULL OR @SeatId IS NULL OR @PriceEcon IS NULL
    RETURN;

IF EXISTS (SELECT 1 FROM [Bookings] WHERE [PnrCode] = N'DEMOCKI1')
    RETURN;

INSERT INTO [Bookings] ([PnrCode], [BookingStatus], [BookingChannel], [CustomerAccountId], [CreatedByAccountId], [ContactFullName], [ContactEmail], [ContactPhoneNumber], [TotalAmount], [ExpiresAt], [CreatedAt], [UpdatedAt])
VALUES (N'DEMOCKI1', N'Paid', N'Website', @CustomerId, @CustomerId, N'Khach Hang Demo', N'customer@airline.com', N'0900000002', @PriceEcon, '2026-01-01T01:00:00', '2026-01-01T00:00:00', NULL);

DECLARE @BookingId INT = SCOPE_IDENTITY();

INSERT INTO [BookingFlights] ([BookingId], [FlightId], [LegOrder], [SeatClass], [Price], [CreatedAt], [UpdatedAt])
VALUES (@BookingId, @FlightId, 1, N'Economy', @PriceEcon, '2026-01-01T00:00:00', NULL);

DECLARE @BookingFlightId INT = SCOPE_IDENTITY();

INSERT INTO [Passengers] ([BookingId], [FullName], [Gender], [PassengerType], [DateOfBirth], [CreatedAt], [UpdatedAt])
VALUES (@BookingId, N'Khach Hang Demo', N'Female', N'Adult', NULL, '2026-01-01T00:00:00', NULL);

DECLARE @PassengerId INT = SCOPE_IDENTITY();

INSERT INTO [Tickets] ([BookingId], [PassengerId], [BookingFlightId], [FlightId], [SeatId], [SeatClass], [Price], [TicketStatus], [AllowCancellation], [IsCheckedIn], [CreatedAt], [UpdatedAt])
VALUES (@BookingId, @PassengerId, @BookingFlightId, @FlightId, NULL, N'Economy', @PriceEcon, N'Paid', 1, 0, '2026-01-01T00:00:00', NULL);

INSERT INTO [Payments] ([BookingId], [PaymentMethod], [Amount], [DiscountAmount], [CouponCode], [PaymentStatus], [PaidAt], [PaymentReference], [CreatedAt], [UpdatedAt])
VALUES (@BookingId, N'Card', @PriceEcon, 0, NULL, N'Paid', '2026-01-01T00:30:00', N'CARD****0001', '2026-01-01T00:30:00', NULL);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @BookingId INT = (SELECT TOP 1 [Id] FROM [Bookings] WHERE [PnrCode] = N'DEMOCKI1');
IF @BookingId IS NOT NULL
BEGIN
    DELETE FROM [Payments]        WHERE [BookingId] = @BookingId;
    DELETE FROM [Tickets]         WHERE [BookingId] = @BookingId;
    DELETE FROM [Passengers]      WHERE [BookingId] = @BookingId;
    DELETE FROM [BookingFlights]  WHERE [BookingId] = @BookingId;
    DELETE FROM [Bookings]        WHERE [Id] = @BookingId;
END

DELETE FROM [Seats]        WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] = N'VN9001');
DELETE FROM [FlightPrices] WHERE [FlightId] IN (SELECT [Id] FROM [Flights] WHERE [FlightNumber] = N'VN9001');
DELETE FROM [Flights]      WHERE [FlightNumber] = N'VN9001';
");
        }
    }
}
