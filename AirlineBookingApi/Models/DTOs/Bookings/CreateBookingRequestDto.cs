using System.ComponentModel.DataAnnotations;
using AirlineBookingApi.Constants;

namespace AirlineBookingApi.Models.DTOs.Bookings;

public class CreateBookingRequestDto
{
    [Required]
    [MinLength(1)]
    public List<BookingFlightRequestDto> Flights { get; set; } = new();

    public int? CustomerAccountId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ContactFullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression(AppConstants.VietnamesePhoneRegex, ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string ContactPhoneNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(9)]
    public List<CreatePassengerRequestDto> Passengers { get; set; } = new();
}
