using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.StaffBookings;

public class CreateStaffBookingRequestDto
{
    [Required]
    [MinLength(1)]
    public List<AirlineBookingApi.Models.DTOs.Bookings.BookingFlightRequestDto> Flights { get; set; } = new();

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
    public string ContactPhoneNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(9)]
    public List<CreateStaffBookingPassengerDto> Passengers { get; set; } = new();
}
