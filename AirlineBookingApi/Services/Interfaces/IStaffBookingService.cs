using AirlineBookingApi.Models.DTOs.StaffBookings;

namespace AirlineBookingApi.Services.Interfaces;

public interface IStaffBookingService
{
    Task<CreateStaffBookingResponseDto> CreateStaffBookingAsync(CreateStaffBookingRequestDto request, int currentStaffId, string currentRole);
    Task<CreateStaffBookingResponseDto?> GetStaffBookingByIdAsync(int bookingId);
    Task<List<CreateStaffBookingResponseDto>> SearchStaffBookingsByPnrAsync(string pnr);
}
