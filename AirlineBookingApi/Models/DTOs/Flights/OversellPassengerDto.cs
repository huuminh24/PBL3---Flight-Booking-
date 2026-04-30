namespace AirlineBookingApi.Models.DTOs.Flights;

public class OversellPassengerDto
{
    public int TicketId { get; set; }
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
    public decimal Price { get; set; }
    public DateTime BookingCreatedAt { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;
}
