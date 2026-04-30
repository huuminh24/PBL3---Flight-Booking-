namespace AirlineBookingApi.Models.Entities;

public class Flight : BaseEntity
{
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;

    public int? AirlineId { get; set; }
    public Airline? Airline { get; set; }

    public ICollection<FlightPrice> FlightPrices { get; set; } = new List<FlightPrice>();
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
