namespace AirlineBookingApi.Models.DTOs.CancelRequests;

public class CancelRequestResponseDto
{
    public int CancelRequestId { get; set; }
    public int TicketId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string CancelStatus { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public string? FlightNumber { get; set; }
    public string? DepartureAirport { get; set; }
    public string? ArrivalAirport { get; set; }
    public DateTime? DepartureTime { get; set; }
    public string? SeatClass { get; set; }
    public DateTime? RequestedAt { get; set; }
}
