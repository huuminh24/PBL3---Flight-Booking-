using AirlineBookingApi.Models.DTOs.History;

namespace AirlineBookingApi.Services.Interfaces;

public interface IHistoryService
{
    Task<List<HistoryBookingListItemDto>> GetMyBookingsAsync(int currentAccountId);
    Task<HistoryBookingDetailDto?> GetMyBookingDetailAsync(int bookingId, int currentAccountId);
    Task<List<HistoryBookingListItemDto>> SearchBookingsAsync(HistorySearchRequestDto request);
}
