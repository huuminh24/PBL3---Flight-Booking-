namespace AirlineBookingApi.Models.Entities;

public class Seat : BaseEntity
{
    public int FlightId { get; set; }
    public Flight? Flight { get; set; }

    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
}
