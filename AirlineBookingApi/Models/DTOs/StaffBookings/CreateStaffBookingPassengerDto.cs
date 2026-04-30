using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.StaffBookings;

public class CreateStaffBookingPassengerDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PassengerType { get; set; } = string.Empty;
}
