using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Auth;

public class UpdateMeRequestDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
