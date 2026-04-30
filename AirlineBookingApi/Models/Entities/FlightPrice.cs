namespace AirlineBookingApi.Models.Entities;

public class FlightPrice : BaseEntity
{
    public int FlightId { get; set; }
    public Flight? Flight { get; set; }

    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
