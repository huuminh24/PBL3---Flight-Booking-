namespace AirlineBookingApi.Models.DTOs.StaffBookings;

public class CreateStaffBookingResponseDto
{
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public string BookingChannel { get; set; } = string.Empty;
    public string ContactFullName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int PassengerCount { get; set; }
    public int CreatedByStaffId { get; set; }
    public int? CustomerAccountId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
