using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Flights;

public class FlightSearchRequestDto
{
  [Required]
  [MaxLength(100)]
  public string DepartureAirport { get; set; } = string.Empty;

  [Required]
  [MaxLength(100)]
  public string ArrivalAirport { get; set; } = string.Empty;

  [Required]
  public DateTime DepartureDate { get; set; }

  public DateTime? ReturnDate { get; set; }

  [Range(1, 9)]
  public int PassengerCount { get; set; }

  public int InfantCount { get; set; }

  [Required]
  public string SeatClass { get; set; } = string.Empty;

  public int? DepartureTimeFrom { get; set; }

  public int? DepartureTimeTo { get; set; }

  public List<string>? Airlines { get; set; }
}
