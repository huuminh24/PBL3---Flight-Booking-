using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.Coupons;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Services.Implementations;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;

    public CouponService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ValidateCouponResponseDto> ValidateCouponAsync(string code)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == code.ToUpper().Trim());

        if (coupon is null)
            return new ValidateCouponResponseDto { Code = code, IsValid = false, Message = "Mã giảm giá không tồn tại." };

        if (!coupon.IsActive)
            return new ValidateCouponResponseDto { Code = code, IsValid = false, Message = "Mã giảm giá đã bị vô hiệu hóa." };

        if (coupon.ExpiryDate < DateTime.UtcNow)
            return new ValidateCouponResponseDto { Code = code, IsValid = false, Message = "Mã giảm giá đã hết hạn." };

        if (coupon.CurrentUses >= coupon.MaxUses)
            return new ValidateCouponResponseDto { Code = code, IsValid = false, Message = "Mã giảm giá đã hết lượt sử dụng." };

        return new ValidateCouponResponseDto
        {
            Code = coupon.Code,
            DiscountPercent = coupon.DiscountPercent,
            IsValid = true,
            Message = $"Áp dụng giảm {coupon.DiscountPercent}% thành công!"
        };
    }

    public async Task UseCouponAsync(string code)
    {
        var normalizedCode = code.ToUpper().Trim();

        var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE Coupons
            SET CurrentUses = CurrentUses + 1,
                UpdatedAt   = {DateTime.UtcNow}
            WHERE Code = {normalizedCode}
              AND IsActive = 1
              AND ExpiryDate >= {DateTime.UtcNow}
              AND CurrentUses < MaxUses;
        ");

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("Mã giảm giá đã hết lượt sử dụng hoặc đã hết hạn.");
        }
    }
}
