namespace AirlineBookingApi.Models.DTOs.History;

public class HistoryBookingListItemDto
{
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int PassengerCount { get; set; }
    public List<HistoryFlightItemDto> Flights { get; set; } = new();
}
