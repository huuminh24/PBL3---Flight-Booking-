using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Coupons;

public class ValidateCouponRequestDto
{
    [Required]
    public string Code { get; set; } = string.Empty;
}

public class ValidateCouponResponseDto
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
