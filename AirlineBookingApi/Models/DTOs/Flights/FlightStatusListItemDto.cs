namespace AirlineBookingApi.Models.DTOs.Flights;

/// <summary>
/// Item trong danh sách chuyến bay cho Staff với thông tin sức chứa & oversell.
/// </summary>
public class FlightStatusListItemDto
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Status { get; set; } = string.Empty;

    // Sức chứa thật & vé đã bán cho Economy
    public int EconomyTotalSeats { get; set; }
    public int EconomyMaxAllowedTickets { get; set; }
    public int EconomyBookedTickets { get; set; }
    public int EconomyCheckedIn { get; set; }

    // Sức chứa thật & vé đã bán cho Business
    public int BusinessTotalSeats { get; set; }
    public int BusinessMaxAllowedTickets { get; set; }
    public int BusinessBookedTickets { get; set; }
    public int BusinessCheckedIn { get; set; }

    /// <summary>Số vé bán vượt sức chứa thật (oversell), tính cho cả 2 hạng.</summary>
    public int OversoldTickets { get; set; }

    /// <summary>Tỷ lệ oversell áp dụng (vd 1.10 = 110%).</summary>
    public decimal OverbookingRatio { get; set; }
}
