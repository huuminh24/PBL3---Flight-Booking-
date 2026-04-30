namespace AirlineBookingApi.Models.DTOs.CheckIn;

public class CustomerCheckInBookingDto
{
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public List<CustomerCheckInTicketDto> Tickets { get; set; } = new();
}

public class CustomerCheckInTicketDto
{
    public int TicketId { get; set; }
    public string PassengerFullName { get; set; } = string.Empty;
    public string PassengerType { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
    public bool CanCheckIn { get; set; }
}
