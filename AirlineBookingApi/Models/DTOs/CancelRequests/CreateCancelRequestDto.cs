using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.CancelRequests;

public class CreateCancelRequestDto
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
