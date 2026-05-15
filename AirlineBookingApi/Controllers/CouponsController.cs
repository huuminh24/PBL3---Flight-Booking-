using AirlineBookingApi.Models.DTOs.Coupons;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    /// <summary>
    /// Validate mã giảm giá. Trả về % giảm nếu hợp lệ.
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequestDto request)
    {
        var result = await _couponService.ValidateCouponAsync(request.Code);
        return Ok(result);
    }
}
