using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class FixStaleSeedDataAndPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            // Fix VN9001 check-in demo flight — shift from stale May 1 to Dec 1 2026
            migrationBuilder.Sql(@"
DECLARE @FlightId INT = (SELECT TOP 1 [Id] FROM [Flights] WHERE [FlightNumber] = N'VN9001');
IF @FlightId IS NOT NULL AND EXISTS (SELECT 1 FROM [Flights] WHERE [Id] = @FlightId AND CONVERT(date, [DepartureTime]) < '2026-11-01')
BEGIN
    UPDATE [Flights] SET [DepartureTime] = '2026-12-01T12:00:00', [ArrivalTime] = '2026-12-01T13:30:00' WHERE [Id] = @FlightId;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
