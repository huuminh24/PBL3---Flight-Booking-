using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AirlineBookingApi.Models.DTOs.Payments;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer,Staff")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
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

            var result = await _paymentService.CreatePaymentAsync(request, currentAccountId, roleClaim);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
