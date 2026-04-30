namespace AirlineBookingApi.Models.DTOs.Reports;

/// <summary>
/// DTO trả về kết quả báo cáo doanh thu tổng hợp cho Staff.
/// </summary>
public class RevenueReportResponseDto
{
    // ── Thông tin bộ lọc đã áp dụng ──
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    // ── Chỉ số tổng hợp ──

    /// <summary>Tổng doanh thu (tổng Amount của các Payment thỏa bộ lọc)</summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>Số giao dịch thanh toán</summary>
    public int TotalTransactions { get; set; }

    /// <summary>Tổng số vé liên quan đến các booking đã thanh toán</summary>
    public int TotalTickets { get; set; }

    /// <summary>Doanh thu trung bình trên mỗi giao dịch</summary>
    public decimal AverageRevenuePerTransaction { get; set; }

    // ── Danh sách chi tiết từng giao dịch ──
    public List<RevenueReportItemDto> Items { get; set; } = new();
}
