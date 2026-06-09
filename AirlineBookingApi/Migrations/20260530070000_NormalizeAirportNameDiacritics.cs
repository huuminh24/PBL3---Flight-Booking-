using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeAirportNameDiacritics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Flights SET DepartureAirport = 'Ha Noi'      WHERE DepartureAirport = N'Hà Nội';
UPDATE Flights SET ArrivalAirport   = 'Ha Noi'      WHERE ArrivalAirport   = N'Hà Nội';
UPDATE Flights SET DepartureAirport = 'Da Nang'     WHERE DepartureAirport = N'Đà Nẵng';
UPDATE Flights SET ArrivalAirport   = 'Da Nang'     WHERE ArrivalAirport   = N'Đà Nẵng';
UPDATE Flights SET DepartureAirport = 'Ho Chi Minh' WHERE DepartureAirport = N'TP. Hồ Chí Minh';
UPDATE Flights SET ArrivalAirport   = 'Ho Chi Minh' WHERE ArrivalAirport   = N'TP. Hồ Chí Minh';
UPDATE Flights SET DepartureAirport = 'Can Tho'     WHERE DepartureAirport = N'Cần Thơ';
UPDATE Flights SET ArrivalAirport   = 'Can Tho'     WHERE ArrivalAirport   = N'Cần Thơ';
UPDATE Flights SET DepartureAirport = 'Hue'         WHERE DepartureAirport = N'Huế';
UPDATE Flights SET ArrivalAirport   = 'Hue'         WHERE ArrivalAirport   = N'Huế';
UPDATE Flights SET DepartureAirport = 'Phu Quoc'    WHERE DepartureAirport = N'Phú Quốc';
UPDATE Flights SET ArrivalAirport   = 'Phu Quoc'    WHERE ArrivalAirport   = N'Phú Quốc';
UPDATE Flights SET DepartureAirport = 'Hai Phong'   WHERE DepartureAirport = N'Hải Phòng';
UPDATE Flights SET ArrivalAirport   = 'Hai Phong'   WHERE ArrivalAirport   = N'Hải Phòng';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Flights SET DepartureAirport = N'Hà Nội'      WHERE DepartureAirport = 'Ha Noi';
UPDATE Flights SET ArrivalAirport   = N'Hà Nội'      WHERE ArrivalAirport   = 'Ha Noi';
UPDATE Flights SET DepartureAirport = N'Đà Nẵng'     WHERE DepartureAirport = 'Da Nang';
UPDATE Flights SET ArrivalAirport   = N'Đà Nẵng'     WHERE ArrivalAirport   = 'Da Nang';
UPDATE Flights SET DepartureAirport = N'TP. Hồ Chí Minh' WHERE DepartureAirport = 'Ho Chi Minh';
UPDATE Flights SET ArrivalAirport   = N'TP. Hồ Chí Minh' WHERE ArrivalAirport   = 'Ho Chi Minh';
UPDATE Flights SET DepartureAirport = N'Cần Thơ'     WHERE DepartureAirport = 'Can Tho';
UPDATE Flights SET ArrivalAirport   = N'Cần Thơ'     WHERE ArrivalAirport   = 'Can Tho';
UPDATE Flights SET DepartureAirport = N'Huế'         WHERE DepartureAirport = 'Hue';
UPDATE Flights SET ArrivalAirport   = N'Huế'         WHERE ArrivalAirport   = 'Hue';
UPDATE Flights SET DepartureAirport = N'Phú Quốc'    WHERE DepartureAirport = 'Phu Quoc';
UPDATE Flights SET ArrivalAirport   = N'Phú Quốc'    WHERE ArrivalAirport   = 'Phu Quoc';
UPDATE Flights SET DepartureAirport = N'Hải Phòng'   WHERE DepartureAirport = 'Hai Phong';
UPDATE Flights SET ArrivalAirport   = N'Hải Phòng'   WHERE ArrivalAirport   = 'Hai Phong';
");
        }
    }
}