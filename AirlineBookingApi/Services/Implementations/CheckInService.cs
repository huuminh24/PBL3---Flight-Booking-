using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.CheckIn;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AirlineBookingApi.Services.Implementations;

public class CheckInService : ICheckInService
{
    private readonly AppDbContext _context;

    public CheckInService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CheckInResponseDto> CheckInTicketAsync(CheckInRequestDto request, string currentRole, int currentAccountId)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var ticket = await _context.Tickets
                .Include(t => t.Passenger)
                .Include(t => t.Booking)
                .Include(t => t.Flight!)
                    .ThenInclude(f => f!.Seats)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId);

            if (ticket is null)
                throw new InvalidOperationException("Không tìm thấy vé.");

            if (ticket.TicketStatus != AppConstants.PaidStatus)
                throw new InvalidOperationException($"Vé chưa được thanh toán (trạng thái hiện tại: {ticket.TicketStatus}). Chỉ vé đã thanh toán mới check-in được.");

            if (ticket.IsCheckedIn)
                throw new InvalidOperationException("Vé này đã được check-in trước đó.");

            var flight = ticket.Flight;

            if (currentRole == AppConstants.CustomerRoleName)
            {
                if (ticket.Booking?.CustomerAccountId != currentAccountId)
                    throw new InvalidOperationException("Bạn chỉ được check-in vé của chính mình.");

                if (flight is not null)
                {
                    var now = DateTime.UtcNow;
                    var windowOpen = flight.DepartureTime.AddHours(-AppConstants.CustomerCheckInMaxHours);
                    var windowClose = flight.DepartureTime.AddHours(-AppConstants.CustomerCheckInMinHours);

                    if (now < windowOpen)
                        throw new InvalidOperationException($"Chưa đến thời gian check-in. Bạn có thể check-in từ {AppConstants.CustomerCheckInMaxHours} giờ trước giờ bay.");
                    if (now > windowClose)
                        throw new InvalidOperationException($"Đã quá thời gian check-in online (trước {AppConstants.CustomerCheckInMinHours} giờ trước giờ bay). Vui lòng check-in tại quầy.");
                }
            }

            var seat = flight?.Seats.FirstOrDefault(s => s.Id == request.SeatId);
            if (seat == null)
                throw new InvalidOperationException("Ghế không tồn tại trên chuyến bay này.");

            if (!seat.IsAvailable)
                throw new InvalidOperationException("Ghế này đã được chọn bởi hành khách khác.");

            if (!string.Equals(seat.SeatClass, ticket.SeatClass, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Bạn chỉ được chọn ghế hạng {ticket.SeatClass}.");

            seat.IsAvailable = false;
            ticket.SeatId = seat.Id;
            ticket.IsCheckedIn = true;
            ticket.TicketStatus = AppConstants.CheckedInStatus;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new CheckInResponseDto
            {
                TicketId = ticket.Id,
                PassengerFullName = ticket.Passenger?.FullName ?? "",
                SeatNumber = seat.SeatNumber,
                SeatClass = ticket.SeatClass,
                IsCheckedIn = true,
                Message = $"Check-in thành công cho hành khách {ticket.Passenger?.FullName}, ghế {seat.SeatNumber}."
            };
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<CheckInSearchResultDto>> SearchForCheckInAsync(string pnrCode, string currentRole, int currentAccountId)
    {
        if (string.IsNullOrWhiteSpace(pnrCode))
            throw new InvalidOperationException("Vui lòng nhập mã PNR.");

        var query = _context.Bookings
            .Include(b => b.Tickets)
            .ThenInclude(t => t.Passenger)
            .Include(b => b.Tickets)
            .ThenInclude(t => t.Seat)
            .Include(b => b.Tickets)
            .ThenInclude(t => t.Flight)
            .Where(b => b.PnrCode != null && b.PnrCode.Contains(pnrCode));

        if (currentRole == AppConstants.CustomerRoleName)
        {
            query = query.Where(b => b.CustomerAccountId == currentAccountId);
        }

        var bookings = await query.ToListAsync();

        var results = new List<CheckInSearchResultDto>();

        foreach (var b in bookings)
        {
            // Group tickets by flight to represent "segments" as search results, because the DTO assumes one flight per result.
var ticketsByFlight = b.Tickets
      .Where(t => t.Flight != null)
      .GroupBy(t => t.FlightId);

    foreach (var group in ticketsByFlight)
    {
      var flight = group.First().Flight!;

                var (windowStatus, minutesUntilOpen, minutesUntilClose) = ComputeCheckInWindow(flight.DepartureTime);

                results.Add(new CheckInSearchResultDto
                {
                    BookingId = b.Id,
                    PnrCode = b.PnrCode,
                    FlightId = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    DepartureAirport = flight.DepartureAirport,
                    ArrivalAirport = flight.ArrivalAirport,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    BookingStatus = b.BookingStatus,
                    CheckInWindowStatus = windowStatus,
                    MinutesUntilOpen = minutesUntilOpen,
                    MinutesUntilClose = minutesUntilClose,
                    Tickets = group.Select(t => new CheckInTicketDto
                    {
                        TicketId = t.Id,
                        FlightId = flight.Id,
                        PassengerFullName = t.Passenger?.FullName ?? "",
                        PassengerType = t.Passenger?.PassengerType ?? "",
                        SeatNumber = t.Seat?.SeatNumber ?? "",
                        SeatClass = t.SeatClass,
                        TicketStatus = t.TicketStatus,
                        IsCheckedIn = t.IsCheckedIn
                    }).ToList()
                });
            }
        }

        return results;
    }

    private static (string status, int minutesUntilOpen, int minutesUntilClose) ComputeCheckInWindow(DateTime departureTimeUtc)
    {
        var now = DateTime.UtcNow;
        var windowOpen = departureTimeUtc.AddHours(-AppConstants.CustomerCheckInMaxHours);
        var windowClose = departureTimeUtc.AddHours(-AppConstants.CustomerCheckInMinHours);

        var minutesUntilOpen = (int)Math.Ceiling((windowOpen - now).TotalMinutes);
        var minutesUntilClose = (int)Math.Ceiling((windowClose - now).TotalMinutes);

        string status;
        if (now < windowOpen)
        {
            status = AppConstants.CheckInWindowNotYetOpen;
        }
        else if (now > windowClose)
        {
            status = AppConstants.CheckInWindowClosed;
        }
        else
        {
            status = AppConstants.CheckInWindowOpen;
        }

        return (status, minutesUntilOpen, minutesUntilClose);
    }
}
