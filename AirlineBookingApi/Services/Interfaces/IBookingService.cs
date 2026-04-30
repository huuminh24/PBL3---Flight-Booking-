using AirlineBookingApi.Models.DTOs.Bookings;

namespace AirlineBookingApi.Services.Interfaces;

public interface IBookingService
{
    Task<CreateBookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request, int currentAccountId, string currentRole);

    /// <summary>Tự động huỷ những booking PendingPayment đã quá ExpiresAt và giải phóng ghế.</summary>
    Task<int> ReleaseExpiredBookingsAsync();

    /// <summary>Lấy hoá đơn của booking; Customer chỉ được xem booking của mình.</summary>
    Task<InvoiceDto?> GetInvoiceAsync(int bookingId, int currentAccountId, string currentRole);
}
