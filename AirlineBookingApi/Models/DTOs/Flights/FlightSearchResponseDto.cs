namespace AirlineBookingApi.Models.DTOs.Flights;

public class FlightSearchResponseDto
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableSeatCount { get; set; }

    public string? AirlineCode { get; set; }
    public string? AirlineName { get; set; }
    public string? AirlineLogoColor { get; set; }
}
