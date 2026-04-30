using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.CheckIn;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/checkin")]
[Authorize(Roles = "Customer,Staff")]
public class CheckInController : ControllerBase
{
    private readonly ICheckInService _checkInService;

    public CheckInController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    private (int accountId, string role) GetCurrentUser()
    {
        var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var accountId))
            throw new UnauthorizedAccessException("Không xác định được tài khoản hiện tại.");

        return (accountId, roleClaim);
    }

    /// <summary>
    /// Check-in một vé. Customer chỉ check-in vé của mình (có cửa sổ thời gian). Staff check-in bất kỳ vé nào.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
    {
        try
        {
            var (accountId, role) = GetCurrentUser();
            var result = await _checkInService.CheckInTicketAsync(request, role, accountId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tìm booking theo PNR để check-in. Customer chỉ thấy booking của mình.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchForCheckIn([FromQuery] string pnr)
    {
        try
        {
            var (accountId, role) = GetCurrentUser();
            var result = await _checkInService.SearchForCheckInAsync(pnr, role, accountId);

            if (!result.Any())
                return NotFound(new { message = "Không tìm thấy booking theo PNR." });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
