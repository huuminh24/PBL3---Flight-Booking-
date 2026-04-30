using AirlineBookingApi.Models.DTOs.CancelRequests;

namespace AirlineBookingApi.Services.Interfaces;

public interface ICancelRequestService
{
    Task<CancelRequestResponseDto> CreateCancelRequestAsync(CreateCancelRequestDto request, int currentAccountId, string currentRole);
    Task<CancelRequestResponseDto> ProcessCancelRequestAsync(int cancelRequestId, ProcessCancelRequestDto request, int currentStaffId, string currentRole);
    Task<List<CancelRequestResponseDto>> GetPendingCancelRequestsAsync(string currentRole);
}
