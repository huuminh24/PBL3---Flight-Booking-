using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Flights;

public class MultiCityLegRequestDto
{
  [Required]
  [MaxLength(100)]
  public string DepartureAirport { get; set; } = string.Empty;

  [Required]
  [MaxLength(100)]
  public string ArrivalAirport { get; set; } = string.Empty;

  [Required]
  public DateTime DepartureDate { get; set; }

  [Required]
  public string SeatClass { get; set; } = string.Empty;
}

public class MultiCitySearchRequestDto
{
  [Required]
  [MinLength(2, ErrorMessage = "Ít nhất 2 chặng bay.")]
  [MaxLength(6, ErrorMessage = "Tối đa 6 chặng bay.")]
  public List<MultiCityLegRequestDto> Legs { get; set; } = new();

  [Range(1, 9)]
  public int PassengerCount { get; set; }

  public int InfantCount { get; set; }

  public int? DepartureTimeFrom { get; set; }
  public int? DepartureTimeTo { get; set; }
  public List<string>? Airlines { get; set; }
}

public class MultiCitySearchResponseDto
{
  public List<List<FlightSearchResponseDto>> LegResults { get; set; } = new();
}