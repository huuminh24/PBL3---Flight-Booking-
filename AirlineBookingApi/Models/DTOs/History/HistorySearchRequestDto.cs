namespace AirlineBookingApi.Models.DTOs.History;

public class HistorySearchRequestDto
{
    public string? PnrCode { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
