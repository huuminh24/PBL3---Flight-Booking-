using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanSeatsAndExpandTo84 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Remove old HasData seed entries (legacy E01-E05, B01-B03 per flight)
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 4);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 5);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 6);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 7);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 8);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 9);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 10);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 11);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 12);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 13);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 14);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 15);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 16);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 17);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 18);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 19);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 20);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 21);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 22);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 23);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 24);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 25);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 26);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 27);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 28);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 29);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 30);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 31);
            migrationBuilder.DeleteData(table: "Seats", keyColumn: "Id", keyValue: 32);

            // Step 2: Clear all remaining seats (legacy TE/TB/E##/B## from other migrations)
            //         and re-seed with 84 modern seats per flight
            migrationBuilder.Sql(@"
-- Gỡ liên kết SeatId từ Tickets đã check-in
UPDATE Tickets SET SeatId = NULL WHERE SeatId IS NOT NULL;

-- Xóa TOÀN BỘ ghế cũ (legacy E01-E50, B01-B18, TE/TB top-up, modern cũ E1A-E9F, B1A-B7B)
DELETE FROM Seats;

-- Seed 84 ghế modern cho MỌI chuyến bay theo layout A320:
--   Business: 6 rows x 2 cols (A-B) = 12  (B1A -> B6B)
--   Economy:  12 rows x 6 cols (A-F) = 72  (E1A -> E12F)
DECLARE @flightId INT;
DECLARE @row INT;
DECLARE @col INT;
DECLARE @sn NVARCHAR(10);

DECLARE flightCur CURSOR FAST_FORWARD FOR SELECT Id FROM Flights;
OPEN flightCur;
FETCH NEXT FROM flightCur INTO @flightId;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Business: rows 1-6, columns A-B (2 per row = 12 seats)
    SET @row = 1;
    WHILE @row <= 6
    BEGIN
        SET @col = 1;
        WHILE @col <= 2
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

            // Step 3: Sync other HasData changes (passwords, flight times) from previous migrations
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$V8uj4i.kgHj7de5nIOw8Uecocdz05pMroStyoy5pXKYWTINmNlsEW");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$jbGC1/QdSVzglrwxS/q3neeL8jmIynxW7YzQkFF7pkt4mADG67EeC");

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 5, 1, 9, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 8, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 5, 1, 14, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 13, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 5, 2, 11, 25, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 5, 2, 17, 50, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 2, 16, 30, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$h0KaW5rACg281rsUC/Eie.iafdih1HoPIh.pgN0aaXFqVFQKAhTqK");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$hvg38hsVfGUQ9exQRfuluOKqXHJpLf1IWSN/DiW9wP1.hm0eGo8tm");

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 12, 1, 9, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 8, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 12, 1, 14, 20, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 1, 13, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 12, 2, 11, 25, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 2, 10, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Flights",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ArrivalTime", "DepartureTime" },
                values: new object[] { new DateTime(2026, 12, 2, 17, 50, 0, 0, DateTimeKind.Utc), new DateTime(2026, 12, 2, 16, 30, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "CreatedAt", "FlightId", "IsAvailable", "SeatClass", "SeatNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Economy", "E01", null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Economy", "E02", null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, "Economy", "E03", null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Economy", "E04", null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Economy", "E05", null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Business", "B01", null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, false, "Business", "B02", null },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "Business", "B03", null },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Economy", "E01", null },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Economy", "E02", null },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Economy", "E03", null },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Economy", "E04", null },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, false, "Economy", "E05", null },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Business", "B01", null },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Business", "B02", null },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "Business", "B03", null },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Economy", "E01", null },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, "Economy", "E02", null },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Economy", "E03", null },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Economy", "E04", null },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Economy", "E05", null },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, false, "Business", "B01", null },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Business", "B02", null },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "Business", "B03", null },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Economy", "E01", null },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Economy", "E02", null },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Economy", "E03", null },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, "Economy", "E04", null },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Economy", "E05", null },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Business", "B01", null },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, false, "Business", "B02", null },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "Business", "B03", null }
                });
        }
    }
}
