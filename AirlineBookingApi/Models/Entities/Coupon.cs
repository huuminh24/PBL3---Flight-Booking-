namespace AirlineBookingApi.Models.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public int MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}
