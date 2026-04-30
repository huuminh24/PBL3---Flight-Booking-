using AirlineBookingApi.Models.DTOs.Payments;

namespace AirlineBookingApi.Services.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request, int currentAccountId, string currentRole);
}
