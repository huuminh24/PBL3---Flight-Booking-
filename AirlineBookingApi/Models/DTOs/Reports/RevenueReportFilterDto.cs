namespace AirlineBookingApi.Models.DTOs.Reports;

/// <summary>
/// DTO chứa các bộ lọc khi Staff yêu cầu báo cáo doanh thu.
/// Tất cả field đều optional — nếu không truyền thì không lọc theo tiêu chí đó.
/// </summary>
public class RevenueReportFilterDto
{
    /// <summary>Từ ngày (lọc theo ngày thanh toán PaidAt)</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Đến ngày</summary>
    public DateTime? ToDate { get; set; }

    /// <summary>Trạng thái thanh toán: Paid, Failed</summary>
    public string? PaymentStatus { get; set; }

    /// <summary>Kênh bán vé: Website, Staff</summary>
    public string? BookingChannel { get; set; }

    /// <summary>Phương thức thanh toán: Cash, BankTransfer, MockGateway</summary>
    public string? PaymentMethod { get; set; }

    /// <summary>Sân bay đi — lọc theo tuyến bay</summary>
    public string? DepartureAirport { get; set; }

    /// <summary>Sân bay đến — lọc theo tuyến bay</summary>
    public string? ArrivalAirport { get; set; }
}
