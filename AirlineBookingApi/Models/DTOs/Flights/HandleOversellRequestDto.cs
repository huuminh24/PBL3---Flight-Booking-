using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Flights;

public class HandleOversellRequestDto
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? StaffNote { get; set; }
}
