using AirlineBookingApi.Models.DTOs.Coupons;

namespace AirlineBookingApi.Services.Interfaces;

public interface ICouponService
{
    Task<ValidateCouponResponseDto> ValidateCouponAsync(string code);
    Task UseCouponAsync(string code);
}
