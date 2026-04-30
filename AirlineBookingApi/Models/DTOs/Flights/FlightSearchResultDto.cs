using System.Collections.Generic;

namespace AirlineBookingApi.Models.DTOs.Flights;

public class FlightSearchResultDto
{
    public List<FlightSearchResponseDto> OutboundFlights { get; set; } = new();
    public List<FlightSearchResponseDto> ReturnFlights { get; set; } = new();
}
