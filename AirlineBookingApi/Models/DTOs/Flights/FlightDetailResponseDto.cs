namespace AirlineBookingApi.Models.DTOs.Flights;

public class FlightDetailResponseDto
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<FlightPriceItemDto> Prices { get; set; } = new();
}

public class FlightPriceItemDto
{
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
