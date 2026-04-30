namespace AirlineBookingApi.Models.DTOs.Payments;

public class CreatePaymentResponseDto
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Amount { get; set; }
    public string? CouponCode { get; set; }
    public DateTime PaidAt { get; set; }
    public string? PaymentReference { get; set; }
}
