namespace AirlineBookingApi.Models.DTOs.Reports;

/// <summary>
/// DTO chi tiết cho từng giao dịch thanh toán trong báo cáo.
/// </summary>
public class RevenueReportItemDto
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;            // VD: "Da Nang → Ho Chi Minh"
    public string BookingChannel { get; set; } = string.Empty;   // Website / Staff
    public string PaymentMethod { get; set; } = string.Empty;    // Cash / BankTransfer / VietQR / Card
    public string PaymentStatus { get; set; } = string.Empty;    // Paid / Failed
    public decimal Amount { get; set; }
    public int TicketCount { get; set; }
    public DateTime PaidAt { get; set; }
}
