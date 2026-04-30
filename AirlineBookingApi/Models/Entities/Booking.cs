namespace AirlineBookingApi.Models.Entities;

public class Booking : BaseEntity
{
    public string PnrCode { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public string BookingChannel { get; set; } = string.Empty;

    public ICollection<BookingFlight> BookingFlights { get; set; } = new List<BookingFlight>();

    public int? CustomerAccountId { get; set; }
    public Account? CustomerAccount { get; set; }

    public int CreatedByAccountId { get; set; }
    public Account? CreatedByAccount { get; set; }

    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    /// <summary>Hạn chót thanh toán; sau thời điểm này booking sẽ tự huỷ.</summary>
    public DateTime ExpiresAt { get; set; }

    public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
