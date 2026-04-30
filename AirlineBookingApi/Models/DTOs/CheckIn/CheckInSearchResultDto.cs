namespace AirlineBookingApi.Models.DTOs.CheckIn;

public class CheckInSearchResultDto
{
    public int BookingId { get; set; }
    public int FlightId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public List<CheckInTicketDto> Tickets { get; set; } = new();

    /// <summary>NotYetOpen | Open | Closed (xem AppConstants.CheckInWindow*).</summary>
    public string CheckInWindowStatus { get; set; } = string.Empty;
    /// <summary>Số phút đến lúc cửa sổ check-in mở; có thể âm nếu đã mở.</summary>
    public int MinutesUntilOpen { get; set; }
    /// <summary>Số phút đến lúc cửa sổ check-in đóng; âm nghĩa là đã đóng.</summary>
    public int MinutesUntilClose { get; set; }
}

public class CheckInTicketDto
{
    public int TicketId { get; set; }
    public int FlightId { get; set; }
    public string PassengerFullName { get; set; } = string.Empty;
    public string PassengerType { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
}
