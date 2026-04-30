namespace AirlineBookingApi.Models.Entities;

public class BookingFlight : BaseEntity
{
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public int FlightId { get; set; }
    public Flight? Flight { get; set; }

    public int LegOrder { get; set; }
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
