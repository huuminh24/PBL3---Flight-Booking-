namespace AirlineBookingApi.Models.DTOs.History;

public class HistoryBookingDetailDto
{
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<HistoryFlightItemDto> Flights { get; set; } = new();
    public List<HistoryTicketItemDto> Tickets { get; set; } = new();
}
