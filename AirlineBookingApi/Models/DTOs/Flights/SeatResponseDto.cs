namespace AirlineBookingApi.Models.DTOs.Flights;

public class SeatResponseDto
{
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
