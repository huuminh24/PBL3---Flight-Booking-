namespace AirlineBookingApi.Models.DTOs.History;

public class HistoryTicketItemDto
{
    public int TicketId { get; set; }
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string PassengerFullName { get; set; } = string.Empty;
    public string PassengerType { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string TicketStatus { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
}
