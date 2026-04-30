using AirlineBookingApi.Models.DTOs.Reports;

namespace AirlineBookingApi.Services.Interfaces;

public interface IReportService
{
    /// <summary>
    /// Lấy báo cáo doanh thu theo bộ lọc.
    /// Chỉ Staff mới được gọi.
    /// </summary>
    Task<RevenueReportResponseDto> GetRevenueReportAsync(RevenueReportFilterDto filter, string currentRole);
}
