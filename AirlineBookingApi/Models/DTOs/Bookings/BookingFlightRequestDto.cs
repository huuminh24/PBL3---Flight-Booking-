using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Bookings;

public class BookingFlightRequestDto
{
    [Required]
    public int FlightId { get; set; }

    [Required]
    public string SeatClass { get; set; } = string.Empty;
}
