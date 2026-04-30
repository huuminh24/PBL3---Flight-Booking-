using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineBookingApi.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedTestFlightsDuplicates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ;WITH cte AS (
                    SELECT
                        Id,
                        ROW_NUMBER() OVER (PARTITION BY FlightId, SeatClass ORDER BY Id) AS rn
                    FROM FlightPrices
                )
                DELETE FROM cte WHERE rn > 1;
            ");

            migrationBuilder.Sql(@"
                ;WITH cte AS (
                    SELECT
                        Id,
                        ROW_NUMBER() OVER (PARTITION BY FlightId, SeatNumber ORDER BY Id) AS rn
                    FROM Seats
                )
                DELETE FROM cte WHERE rn > 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op (data cleanup)
        }
    }
}
