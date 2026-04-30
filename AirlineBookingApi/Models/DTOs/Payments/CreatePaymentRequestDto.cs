using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Payments;

public class CreatePaymentRequestDto
{
    public int? BookingId { get; set; }

    [MaxLength(20)]
    public string? PnrCode { get; set; }

    [Required]
    [MaxLength(30)]
    public string PaymentMethod { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }

    [MaxLength(50)]
    public string? CouponCode { get; set; }

    /// <summary>4 số cuối thẻ tín dụng (chỉ lưu chứng từ giả lập).</summary>
    [MaxLength(4)]
    public string? CardLast4 { get; set; }

    /// <summary>4 số cuối tài khoản ngân hàng đã chuyển khoản.</summary>
    [MaxLength(4)]
    public string? BankAccountLast4 { get; set; }

    /// <summary>Mã giao dịch QR (tự sinh trên client để giả lập).</summary>
    [MaxLength(50)]
    public string? QrTransactionId { get; set; }
}
