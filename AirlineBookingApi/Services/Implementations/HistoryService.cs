using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.History;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Services.Implementations;

public class HistoryService : IHistoryService
{
    private readonly AppDbContext _dbContext;

    public HistoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<HistoryBookingListItemDto>> GetMyBookingsAsync(int currentAccountId)
    {
        return await _dbContext.Bookings
            .Include(x => x.BookingFlights)
                .ThenInclude(bf => bf.Flight)
            .Include(x => x.Passengers)
            .Where(x => x.CustomerAccountId == currentAccountId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new HistoryBookingListItemDto
            {
                BookingId = x.Id,
                PnrCode = x.PnrCode,
                BookingStatus = x.BookingStatus,
                TotalAmount = x.TotalAmount,
                PassengerCount = x.Passengers.Count,
                Flights = x.BookingFlights.OrderBy(bf => bf.LegOrder).Select(bf => new HistoryFlightItemDto
                {
                    FlightId = bf.Flight!.Id,
                    FlightNumber = bf.Flight.FlightNumber,
                    DepartureAirport = bf.Flight.DepartureAirport,
                    ArrivalAirport = bf.Flight.ArrivalAirport,
                    DepartureTime = bf.Flight.DepartureTime,
                    ArrivalTime = bf.Flight.ArrivalTime,
                    FlightStatus = bf.Flight.Status,
                    SeatClass = bf.SeatClass,
                    LegOrder = bf.LegOrder
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<HistoryBookingDetailDto?> GetMyBookingDetailAsync(int bookingId, int currentAccountId)
    {
        var booking = await _dbContext.Bookings
            .Include(x => x.BookingFlights)
                .ThenInclude(bf => bf.Flight)
            .Include(x => x.Tickets)
                .ThenInclude(x => x.Passenger)
            .Include(x => x.Tickets)
                .ThenInclude(x => x.Seat)
            .Include(x => x.Tickets)
                .ThenInclude(x => x.Flight)
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking is null)
        {
            return null;
        }

        if (booking.CustomerAccountId != currentAccountId)
        {
            throw new InvalidOperationException("Customer không được xem booking của người khác.");
        }

        return new HistoryBookingDetailDto
        {
            BookingId = booking.Id,
            PnrCode = booking.PnrCode,
            BookingStatus = booking.BookingStatus,
            ContactFullName = booking.ContactFullName,
            ContactEmail = booking.ContactEmail,
            ContactPhoneNumber = booking.ContactPhoneNumber,
            TotalAmount = booking.TotalAmount,
            Flights = booking.BookingFlights.OrderBy(bf => bf.LegOrder).Select(bf => new HistoryFlightItemDto
            {
                FlightId = bf.Flight!.Id,
                FlightNumber = bf.Flight.FlightNumber,
                DepartureAirport = bf.Flight.DepartureAirport,
                ArrivalAirport = bf.Flight.ArrivalAirport,
                DepartureTime = bf.Flight.DepartureTime,
                ArrivalTime = bf.Flight.ArrivalTime,
                FlightStatus = bf.Flight.Status,
                SeatClass = bf.SeatClass,
                LegOrder = bf.LegOrder
            }).ToList(),
            Tickets = booking.Tickets.Select(ticket => new HistoryTicketItemDto
            {
                TicketId = ticket.Id,
                FlightId = ticket.Flight!.Id,
                FlightNumber = ticket.Flight.FlightNumber,
                PassengerFullName = ticket.Passenger!.FullName,
                PassengerType = ticket.Passenger.PassengerType,
                SeatNumber = ticket.Seat?.SeatNumber ?? "Chưa chọn",
                SeatClass = ticket.SeatClass,
                Price = ticket.Price,
                TicketStatus = ticket.TicketStatus,
                IsCheckedIn = ticket.IsCheckedIn
            }).ToList()
        };
    }

    public async Task<List<HistoryBookingListItemDto>> SearchBookingsAsync(HistorySearchRequestDto request)
    {
        var query = _dbContext.Bookings
            .Include(x => x.BookingFlights)
                .ThenInclude(bf => bf.Flight)
            .Include(x => x.Passengers)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.PnrCode))
        {
            query = query.Where(x => x.PnrCode == request.PnrCode.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(x => x.ContactEmail == request.Email.Trim());
        }

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(x => x.ContactPhoneNumber == request.PhoneNumber.Trim());
        }

        var page = request.Page.GetValueOrDefault(1);
        var pageSize = request.PageSize.GetValueOrDefault(50);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 200) pageSize = 200;

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new HistoryBookingListItemDto
            {
                BookingId = x.Id,
                PnrCode = x.PnrCode,
                BookingStatus = x.BookingStatus,
                TotalAmount = x.TotalAmount,
                PassengerCount = x.Passengers.Count,
                Flights = x.BookingFlights.OrderBy(bf => bf.LegOrder).Select(bf => new HistoryFlightItemDto
                {
                    FlightId = bf.Flight!.Id,
                    FlightNumber = bf.Flight.FlightNumber,
                    DepartureAirport = bf.Flight.DepartureAirport,
                    ArrivalAirport = bf.Flight.ArrivalAirport,
                    DepartureTime = bf.Flight.DepartureTime,
                    ArrivalTime = bf.Flight.ArrivalTime,
                    FlightStatus = bf.Flight.Status,
                    SeatClass = bf.SeatClass,
                    LegOrder = bf.LegOrder
                }).ToList()
            })
            .ToListAsync();
    }
}
