namespace AirlineBookingApi.Models.Entities;

public class Ticket : BaseEntity
{
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public int PassengerId { get; set; }
    public Passenger? Passenger { get; set; }

    public int BookingFlightId { get; set; }
    public BookingFlight? BookingFlight { get; set; }

    public int FlightId { get; set; }
    public Flight? Flight { get; set; }

    public int? SeatId { get; set; }
    public Seat? Seat { get; set; }

    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string TicketStatus { get; set; } = string.Empty;
    public bool AllowCancellation { get; set; } = true;
    public bool IsCheckedIn { get; set; }

    public ICollection<CancelRequest> CancelRequests { get; set; } = new List<CancelRequest>();
}
