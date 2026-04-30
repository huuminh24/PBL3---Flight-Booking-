using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.CancelRequests;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer,Staff")]
public class CancelRequestsController : ControllerBase
{
    private readonly ICancelRequestService _cancelRequestService;

    public CancelRequestsController(ICancelRequestService cancelRequestService)
    {
        _cancelRequestService = cancelRequestService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCancelRequest([FromBody] CreateCancelRequestDto request)
    {
        try
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var currentAccountId))
            {
                return Unauthorized(new { message = "Không xác định được tài khoản hiện tại." });
            }

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                return Unauthorized(new { message = "Không xác định được role hiện tại." });
            }

            var result = await _cancelRequestService.CreateCancelRequestAsync(request, currentAccountId, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Staff xem danh sách yêu cầu hủy đang chờ duyệt.
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetPendingCancelRequests()
    {
        try
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                return Unauthorized(new { message = "Không xác định được role hiện tại." });
            }

            var result = await _cancelRequestService.GetPendingCancelRequestsAsync(roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Staff duyệt hoặc từ chối yêu cầu hủy vé.
    /// </summary>
    [HttpPut("{id:int}/process")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> ProcessCancelRequest(int id, [FromBody] ProcessCancelRequestDto request)
    {
        try
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(accountIdClaim) || !int.TryParse(accountIdClaim, out var currentStaffId))
            {
                return Unauthorized(new { message = "Không xác định được tài khoản hiện tại." });
            }

            if (string.IsNullOrWhiteSpace(roleClaim))
            {
                return Unauthorized(new { message = "Không xác định được role hiện tại." });
            }

            var result = await _cancelRequestService.ProcessCancelRequestAsync(id, request, currentStaffId, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
