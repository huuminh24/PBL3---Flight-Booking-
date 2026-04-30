namespace AirlineBookingApi.Models.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }

    /// <summary>Tham chiếu chứng từ giả lập: last4 thẻ, last4 STK ngân hàng hoặc mã giao dịch QR.</summary>
    public string? PaymentReference { get; set; }
}
