using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.History;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer,Staff")]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet("my-bookings")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookings()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var currentAccountId))
        {
            return Unauthorized(new { message = "Không xác định được tài khoản hiện tại." });
        }

        var result = await _historyService.GetMyBookingsAsync(currentAccountId);
        return Ok(result);
    }

    [HttpGet("my-bookings/{id:int}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookingDetail(int id)
    {
        try
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var currentAccountId))
            {
                return Unauthorized(new { message = "Không xác định được tài khoản hiện tại." });
            }

            var result = await _historyService.GetMyBookingDetailAsync(id, currentAccountId);

            if (result is null)
            {
                return NotFound(new { message = "Không tìm thấy booking." });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("search")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> SearchBookings([FromQuery] HistorySearchRequestDto request)
    {
        var result = await _historyService.SearchBookingsAsync(request);

        return Ok(result);
    }
}
