using AirlineBookingApi.Models.DTOs.Flights;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    // ─── Public GET endpoints ───

    [HttpGet("airports")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAirports()
    {
        var airports = await _flightService.GetAirportsAsync();
        return Ok(airports);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchFlights([FromQuery] FlightSearchRequestDto request)
    {
        try
        {
            var result = await _flightService.SearchFlightsAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("search-multi-city")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchMultiCity([FromBody] MultiCitySearchRequestDto request)
    {
        try
        {
            var result = await _flightService.SearchMultiCityAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{flightId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFlightDetail(int flightId)
    {
        var result = await _flightService.GetFlightDetailAsync(flightId);

        if (result is null)
        {
            return NotFound(new { message = "Không tìm thấy chuyến bay." });
        }

        return Ok(result);
    }

    [HttpGet("{flightId:int}/seats")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeats(int flightId)
    {
        var seats = await _flightService.GetSeatsByFlightAsync(flightId);

        if (!seats.Any())
        {
            return NotFound(new { message = "Không tìm thấy ghế của chuyến bay." });
        }

        return Ok(seats);
    }

    [HttpGet("{flightId:int}/status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFlightStatus(int flightId)
    {
        var result = await _flightService.GetFlightStatusAsync(flightId);

        if (result is null)
        {
            return NotFound(new { message = "Không tìm thấy tình trạng chuyến bay." });
        }

        return Ok(result);
    }

    // ─── Staff endpoints ───

    /// <summary>Staff: liệt kê chuyến bay kèm thông tin sức chứa, lọc theo ngày/route/status.</summary>
    [HttpGet("staff/list")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetStaffFlightList(
        [FromQuery] DateTime? date,
        [FromQuery] string? departureAirport,
        [FromQuery] string? arrivalAirport,
        [FromQuery] string? status)
    {
        var result = await _flightService.GetStaffFlightListAsync(date, departureAirport, arrivalAirport, status);
        return Ok(result);
    }

    [HttpGet("{flightId:int}/oversell-passengers")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetOversellPassengers(int flightId)
    {
        try
        {
            var result = await _flightService.GetOversellPassengersAsync(flightId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{flightId:int}/handle-oversell")]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> HandleOversell(int flightId, [FromBody] HandleOversellRequestDto request)
    {
        try
        {
            var message = await _flightService.HandleOversellAsync(flightId, request);
            return Ok(new { message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
