using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.Reports;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/staff/reports")]
[Authorize(Roles = "Staff")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Lấy báo cáo doanh thu. Chỉ Staff mới có quyền gọi.
    /// Tất cả bộ lọc đều optional, truyền qua query string.
    /// </summary>
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] RevenueReportFilterDto filter)
    {
        try
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                return Unauthorized(new { message = "Không xác định được role hiện tại." });
            }

            var result = await _reportService.GetRevenueReportAsync(filter, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
