namespace AirlineBookingApi.Models.Entities;

public class Airline : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoColor { get; set; }

    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
