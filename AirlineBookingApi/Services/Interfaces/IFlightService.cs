using AirlineBookingApi.Models.DTOs.Flights;

namespace AirlineBookingApi.Services.Interfaces;

public interface IFlightService
{
    Task<FlightSearchResultDto> SearchFlightsAsync(FlightSearchRequestDto request);
  Task<MultiCitySearchResponseDto> SearchMultiCityAsync(MultiCitySearchRequestDto request);
    Task<FlightDetailResponseDto?> GetFlightDetailAsync(int flightId);
    Task<List<SeatResponseDto>> GetSeatsByFlightAsync(int flightId);
    Task<FlightStatusResponseDto?> GetFlightStatusAsync(int flightId);
    Task<List<string>> GetAirportsAsync();

    /// <summary>Staff: liệt kê chuyến bay kèm thông tin sức chứa & oversell, có thể lọc theo ngày/route/status.</summary>
    Task<List<FlightStatusListItemDto>> GetStaffFlightListAsync(DateTime? date, string? departureAirport, string? arrivalAirport, string? status);

    Task<List<OversellPassengerDto>> GetOversellPassengersAsync(int flightId);
    Task<string> HandleOversellAsync(int flightId, HandleOversellRequestDto request);
}
