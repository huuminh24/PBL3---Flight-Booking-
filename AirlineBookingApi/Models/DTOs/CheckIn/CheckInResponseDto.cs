namespace AirlineBookingApi.Models.DTOs.CheckIn;

public class CheckInResponseDto
{
    public int TicketId { get; set; }
    public string PassengerFullName { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
    public string Message { get; set; } = string.Empty;
}
