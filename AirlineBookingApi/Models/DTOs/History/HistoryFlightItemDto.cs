namespace AirlineBookingApi.Models.DTOs.History;

public class HistoryFlightItemDto
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string FlightStatus { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public int LegOrder { get; set; }
}
