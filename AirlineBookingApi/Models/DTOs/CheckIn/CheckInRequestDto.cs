using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.CheckIn;

public class CheckInRequestDto
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    public int SeatId { get; set; }
}
