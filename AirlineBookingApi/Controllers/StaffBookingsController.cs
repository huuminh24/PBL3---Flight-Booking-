using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.StaffBookings;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/staff/bookings")]
[Authorize(Roles = "Staff")]
public class StaffBookingsController : ControllerBase
{
    private readonly IStaffBookingService _staffBookingService;

    public StaffBookingsController(IStaffBookingService staffBookingService)
    {
        _staffBookingService = staffBookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaffBooking([FromBody] CreateStaffBookingRequestDto request)
    {
        try
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var currentStaffId))
            {
                return Unauthorized(new { message = "Không xác định được tài khoản staff hiện tại." });
            }

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                return Unauthorized(new { message = "Không xác định được role hiện tại." });
            }

            var result = await _staffBookingService.CreateStaffBookingAsync(request, currentStaffId, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStaffBookingById(int id)
    {
        var result = await _staffBookingService.GetStaffBookingByIdAsync(id);

        if (result is null)
        {
            return NotFound(new { message = "Không tìm thấy booking hộ khách hàng." });
        }

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByPnr([FromQuery] string pnr)
    {
        var result = await _staffBookingService.SearchStaffBookingsByPnrAsync(pnr);

        if (!result.Any())
        {
            return NotFound(new { message = "Không tìm thấy booking theo PNR." });
        }

        return Ok(result);
    }
}
