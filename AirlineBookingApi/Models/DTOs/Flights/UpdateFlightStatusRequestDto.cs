using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Flights;

/// <summary>
/// Body cho PUT /api/Flights/{id}/status — Staff cập nhật trạng thái chuyến bay.
/// </summary>
public class UpdateFlightStatusRequestDto
{
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}
