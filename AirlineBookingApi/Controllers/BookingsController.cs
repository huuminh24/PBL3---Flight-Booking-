using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.Bookings;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer,Staff")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
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

            var result = await _bookingService.CreateBookingAsync(request, currentAccountId, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy hoá đơn của booking. Customer chỉ xem được booking của mình; Staff xem mọi booking.
    /// </summary>
    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
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

            var invoice = await _bookingService.GetInvoiceAsync(id, currentAccountId, roleClaim);
            if (invoice is null)
            {
                return NotFound(new { message = "Không tìm thấy booking." });
            }

            return Ok(invoice);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
