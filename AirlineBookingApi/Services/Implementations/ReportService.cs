using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.Reports;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Services.Implementations;

public class ReportService : IReportService
{
    private const string StaffRoleName = "Staff";

    private readonly AppDbContext _dbContext;

    public ReportService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RevenueReportResponseDto> GetRevenueReportAsync(RevenueReportFilterDto filter, string currentRole)
    {
        if (!string.Equals(currentRole, StaffRoleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chỉ Staff mới được xem báo cáo doanh thu.");
        }

        if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
        {
            throw new InvalidOperationException("Từ ngày phải nhỏ hơn hoặc bằng đến ngày.");
        }

        var query = _dbContext.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b!.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
            .Include(p => p.Booking)
                .ThenInclude(b => b!.Tickets)
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            query = query.Where(p => p.PaidAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            var toDateEnd = filter.ToDate.Value.Date.AddDays(1);
            query = query.Where(p => p.PaidAt < toDateEnd);
        }

        if (!string.IsNullOrWhiteSpace(filter.PaymentStatus))
        {
            query = query.Where(p => p.PaymentStatus == filter.PaymentStatus.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filter.BookingChannel))
        {
            query = query.Where(p => p.Booking!.BookingChannel == filter.BookingChannel.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
        {
            query = query.Where(p => p.PaymentMethod == filter.PaymentMethod.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filter.DepartureAirport))
        {
            var dep = filter.DepartureAirport.Trim();
            query = query.Where(p => p.Booking!.BookingFlights.Any(bf => bf.Flight!.DepartureAirport == dep));
        }

        if (!string.IsNullOrWhiteSpace(filter.ArrivalAirport))
        {
            var arr = filter.ArrivalAirport.Trim();
            query = query.Where(p => p.Booking!.BookingFlights.Any(bf => bf.Flight!.ArrivalAirport == arr));
        }

        var payments = await query
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();

        var totalRevenue = payments.Sum(p => p.Amount);
        var totalTransactions = payments.Count;
        var totalTickets = payments.Sum(p => p.Booking?.Tickets.Count ?? 0);
        var averageRevenue = totalTransactions > 0
            ? totalRevenue / totalTransactions
            : 0;

        var items = payments.Select(p => 
        {
            var flights = p.Booking?.BookingFlights.OrderBy(bf => bf.LegOrder).Select(bf => bf.Flight).ToList();
            var flightNumbers = flights != null ? string.Join(", ", flights.Select(f => f?.FlightNumber)) : string.Empty;
            
            string route = string.Empty;
            if (flights != null && flights.Any())
            {
                if (flights.Count == 1)
                {
                    route = FormatRoute(flights.First()?.DepartureAirport, flights.First()?.ArrivalAirport);
                }
                else
                {
                    // For multiple flights, we can just join all airports or show start to end
                    route = string.Join(" → ", flights.Select(f => f?.DepartureAirport).Concat(new[] { flights.Last()?.ArrivalAirport }).Distinct());
                }
            }

            return new RevenueReportItemDto
            {
                PaymentId = p.Id,
                BookingId = p.BookingId,
                PnrCode = p.Booking?.PnrCode ?? string.Empty,
                FlightNumber = flightNumbers,
                Route = route,
                BookingChannel = p.Booking?.BookingChannel ?? string.Empty,
                PaymentMethod = p.PaymentMethod,
                PaymentStatus = p.PaymentStatus,
                Amount = p.Amount,
                TicketCount = p.Booking?.Tickets.Count ?? 0,
                PaidAt = p.PaidAt
            };
        }).ToList();

        return new RevenueReportResponseDto
        {
            FromDate = filter.FromDate,
            ToDate = filter.ToDate,
            TotalRevenue = totalRevenue,
            TotalTransactions = totalTransactions,
            TotalTickets = totalTickets,
            AverageRevenuePerTransaction = averageRevenue,
            Items = items
        };
    }

    private static string FormatRoute(string? departure, string? arrival)
    {
        if (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(arrival))
        {
            return string.Empty;
        }

        return $"{departure} → {arrival}";
    }
}
