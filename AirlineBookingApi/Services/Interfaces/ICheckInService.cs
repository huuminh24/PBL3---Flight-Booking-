using AirlineBookingApi.Models.DTOs.CheckIn;

namespace AirlineBookingApi.Services.Interfaces;

public interface ICheckInService
{
    Task<CheckInResponseDto> CheckInTicketAsync(CheckInRequestDto request, string currentRole, int currentAccountId);
    Task<List<CheckInSearchResultDto>> SearchForCheckInAsync(string pnrCode, string currentRole, int currentAccountId);
}
