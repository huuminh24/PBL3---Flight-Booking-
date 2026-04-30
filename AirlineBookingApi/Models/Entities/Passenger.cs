namespace AirlineBookingApi.Models.Entities;

public class Passenger : BaseEntity
{
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string PassengerType { get; set; } = string.Empty;

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
