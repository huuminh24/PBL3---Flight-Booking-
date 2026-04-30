using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingFlightIdToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingFlightId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BookingFlightId",
                table: "Tickets",
                column: "BookingFlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_BookingFlights_BookingFlightId",
                table: "Tickets",
                column: "BookingFlightId",
                principalTable: "BookingFlights",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_BookingFlights_BookingFlightId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_BookingFlightId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BookingFlightId",
                table: "Tickets");
        }
    }
}
