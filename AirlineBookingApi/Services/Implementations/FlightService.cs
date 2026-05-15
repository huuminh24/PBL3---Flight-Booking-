using System.Globalization;
using System.Text;
using AirlineBookingApi.Configurations;
using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.Flights;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AirlineBookingApi.Services.Implementations;

public class FlightService : IFlightService
{
    private readonly AppDbContext _dbContext;
    private readonly decimal _overbookingRatio;
    private readonly ILogger<FlightService> _logger;

    public FlightService(AppDbContext dbContext, IOptions<BookingSettings> bookingOptions, ILogger<FlightService> logger)
    {
        _dbContext = dbContext;
        _overbookingRatio = bookingOptions?.Value?.OverbookingRatio > 0
            ? bookingOptions.Value.OverbookingRatio
            : AppConstants.DefaultOverbookingRatio;
        _logger = logger;
    }

    public async Task<List<string>> GetAirportsAsync()
    {
        var dep = await _dbContext.Flights
            .Select(x => x.DepartureAirport)
            .Distinct()
            .ToListAsync();

        var arr = await _dbContext.Flights
            .Select(x => x.ArrivalAirport)
            .Distinct()
            .ToListAsync();

        return dep
            .Concat(arr)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
    }

    public async Task<FlightSearchResultDto> SearchFlightsAsync(FlightSearchRequestDto request)
    {
        ValidateSearchRequest(request);

        var normalizedDepartureAirport = NormalizeAirportName(request.DepartureAirport);
        var normalizedArrivalAirport = NormalizeAirportName(request.ArrivalAirport);
        var normalizedSeatClass = request.SeatClass.Trim();

        var depTimeFrom = request.DepartureTimeFrom;
        var depTimeTo = request.DepartureTimeTo;
        var airlines = request.Airlines?.Select(a => a.Trim().ToLower()).Where(a => !string.IsNullOrEmpty(a)).ToList();

var outboundFlights = await SearchSingleDirectionAsync(
  normalizedDepartureAirport,
  normalizedArrivalAirport,
  request.DepartureDate.Date,
  normalizedSeatClass,
  request.PassengerCount,
  request.InfantCount,
  depTimeFrom,
  depTimeTo,
  airlines);

var result = new FlightSearchResultDto
{
  OutboundFlights = outboundFlights
};

if (request.ReturnDate.HasValue)
{
  var returnFlights = await SearchSingleDirectionAsync(
  normalizedArrivalAirport,
  normalizedDepartureAirport,
  request.ReturnDate.Value.Date,
  normalizedSeatClass,
  request.PassengerCount,
  request.InfantCount,
  null,
  null,
  airlines);

  result.ReturnFlights = returnFlights;
}

        return result;
    }

private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
      "Vietnam", TimeSpan.FromHours(7), "Vietnam Standard Time", "VST");

  private (DateTime utcStart, DateTime utcEnd) GetUtcDayRange(DateTime localDate)
  {
    var localDayStart = DateTime.SpecifyKind(localDate.Date, DateTimeKind.Unspecified);
    var utcStart = TimeZoneInfo.ConvertTimeToUtc(localDayStart, VietnamTimeZone);
    var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localDayStart.AddDays(1), VietnamTimeZone);
    return (utcStart, utcEnd);
  }

  private static string NormalizeAirportName(string s)
  {
    if (string.IsNullOrWhiteSpace(s)) return string.Empty;
    var normalized = s.Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder();
    foreach (var c in normalized)
    {
      if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        sb.Append(c);
    }
    return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
  }

  private async Task<List<FlightSearchResponseDto>> SearchSingleDirectionAsync(string depAirport, string arrAirport, DateTime date, string seatClass, int paxCount, int infantCount = 0, int? depTimeFrom = null, int? depTimeTo = null, List<string>? airlines = null)
  {
    // Convert local Vietnam date to UTC range
    var (utcDayStart, utcDayEnd) = GetUtcDayRange(date);

  var nowUtc = DateTime.UtcNow;

  var normalizedDep = NormalizeAirportName(depAirport);
  var normalizedArr = NormalizeAirportName(arrAirport);

  var query = _dbContext.Flights
  .Include(x => x.FlightPrices)
  .Include(x => x.Seats)
  .Include(x => x.Airline)
  .Where(x => x.DepartureTime >= utcDayStart && x.DepartureTime < utcDayEnd
  && x.DepartureTime >= nowUtc);

  if (depTimeFrom.HasValue)
  {
    // DepartureTime is UTC; add 7h to compare against Vietnam local hour
    query = query.Where(x => x.DepartureTime.AddHours(7).Hour >= depTimeFrom.Value);
  }

  if (depTimeTo.HasValue)
  {
    query = query.Where(x => x.DepartureTime.AddHours(7).Hour < depTimeTo.Value);
  }

  if (airlines != null && airlines.Count > 0)
  {
    query = query.Where(x => x.Airline != null && airlines.Contains(x.Airline.Code.ToLower()));
  }

  var allFlights = await query.ToListAsync();

  var flights = allFlights
    .Where(x => string.IsNullOrWhiteSpace(normalizedDep) || NormalizeAirportName(x.DepartureAirport) == normalizedDep)
    .Where(x => string.IsNullOrWhiteSpace(normalizedArr) || NormalizeAirportName(x.ArrivalAirport) == normalizedArr)
    .ToList();

  var flightIds = flights.Select(f => f.Id).ToList();

  var bookedCounts = await _dbContext.Tickets
  .Where(t => flightIds.Contains(t.FlightId)
      && t.SeatClass == seatClass
      && t.TicketStatus != AppConstants.CancelledStatus
      && !(t.Booking != null
           && t.Booking.BookingStatus == AppConstants.PendingPaymentStatus
           && t.Booking.ExpiresAt < nowUtc))
  .GroupBy(t => t.FlightId)
  .Select(g => new { FlightId = g.Key, Count = g.Count() })
  .ToDictionaryAsync(x => x.FlightId, x => x.Count);

  var seatsNeeded = Math.Max(1, paxCount - infantCount);

  return flights
  .Select(flight =>
  {
    var totalSeatsInClass = flight.Seats.Count(seat => seat.SeatClass == seatClass);
    var maxAllowedTickets = AppConstants.CalcMaxAllowedTickets(totalSeatsInClass, _overbookingRatio);
    bookedCounts.TryGetValue(flight.Id, out var bookedInClass);
    var availableSeatCount = maxAllowedTickets - bookedInClass;

    var selectedPrice = flight.FlightPrices
    .FirstOrDefault(price => price.SeatClass == seatClass);

    if (selectedPrice is null)
    {
      _logger.LogWarning("[FlightSearch] Chuyến bay {FlightId} ({FlightNumber}) không có giá cho hạng {SeatClass} — bị bỏ qua.", flight.Id, flight.FlightNumber, seatClass);
      return null;
    }

    if (availableSeatCount < seatsNeeded)
    {
      _logger.LogDebug("[FlightSearch] Chuyến bay {FlightId} không đủ chỗ: cần {Needed}, còn {Available}.", flight.Id, seatsNeeded, availableSeatCount);
      return null;
    }

    return new FlightSearchResponseDto
    {
      FlightId = flight.Id,
      FlightNumber = flight.FlightNumber,
      DepartureAirport = flight.DepartureAirport,
      ArrivalAirport = flight.ArrivalAirport,
      DepartureTime = flight.DepartureTime,
      ArrivalTime = flight.ArrivalTime,
      Status = flight.Status,
      SeatClass = seatClass,
      Price = selectedPrice.Price,
      AvailableSeatCount = availableSeatCount,
      AirlineCode = flight.Airline?.Code,
      AirlineName = flight.Airline?.Name,
      AirlineLogoColor = flight.Airline?.LogoColor
    };
  })
  .Where(x => x is not null)
  .Cast<FlightSearchResponseDto>()
  .OrderBy(x => x.DepartureTime)
  .ToList();
}

public async Task<FlightDetailResponseDto?> GetFlightDetailAsync(int flightId)
    {
        var flight = await _dbContext.Flights
            .Include(x => x.FlightPrices)
            .FirstOrDefaultAsync(x => x.Id == flightId);

        if (flight is null)
        {
            return null;
        }

        return new FlightDetailResponseDto
        {
            FlightId = flight.Id,
            FlightNumber = flight.FlightNumber,
            DepartureAirport = flight.DepartureAirport,
            ArrivalAirport = flight.ArrivalAirport,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Status = flight.Status,
            Prices = flight.FlightPrices
                .OrderBy(x => x.Price)
                .Select(x => new FlightPriceItemDto
                {
                    SeatClass = x.SeatClass,
                    Price = x.Price
                })
                .ToList()
        };
    }

    public async Task<List<SeatResponseDto>> GetSeatsByFlightAsync(int flightId)
    {
        var seatList = await _dbContext.Seats
            .Where(x => x.FlightId == flightId)
            .OrderBy(x => x.SeatClass)
            .ThenBy(x => x.SeatNumber)
            .Select(x => new SeatResponseDto
            {
                SeatId = x.Id,
                SeatNumber = x.SeatNumber,
                SeatClass = x.SeatClass,
                IsAvailable = x.IsAvailable
            })
            .ToListAsync();

        return seatList;
    }

    public async Task<FlightStatusResponseDto?> GetFlightStatusAsync(int flightId)
    {
        var flight = await _dbContext.Flights
            .FirstOrDefaultAsync(x => x.Id == flightId);

        if (flight is null)
        {
            return null;
        }

        return new FlightStatusResponseDto
        {
            FlightId = flight.Id,
            FlightNumber = flight.FlightNumber,
            DepartureAirport = flight.DepartureAirport,
            ArrivalAirport = flight.ArrivalAirport,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Status = flight.Status
        };
    }

    public async Task<MultiCitySearchResponseDto> SearchMultiCityAsync(MultiCitySearchRequestDto request)
  {
    ValidateMultiCityRequest(request);

    var legs = request.Legs;

    var legResults = new List<List<FlightSearchResponseDto>>();

    for (int i = 0; i < legs.Count; i++)
    {
      var leg = legs[i];
      var normalizedDep = NormalizeAirportName(leg.DepartureAirport);
      var normalizedArr = NormalizeAirportName(leg.ArrivalAirport);
      var seatClass = leg.SeatClass.Trim();

      var normalizedAirlines = request.Airlines?.Select(a => a.Trim().ToLower()).Where(a => !string.IsNullOrEmpty(a)).ToList();

      var results = await SearchSingleDirectionAsync(
        normalizedDep,
        normalizedArr,
        leg.DepartureDate.Date,
        seatClass,
        request.PassengerCount,
        request.InfantCount,
        request.DepartureTimeFrom,
        request.DepartureTimeTo,
        normalizedAirlines);

      legResults.Add(results);
    }

    return new MultiCitySearchResponseDto { LegResults = legResults };
  }

  private static void ValidateMultiCityRequest(MultiCitySearchRequestDto request)
  {
    if (request.Legs is null || request.Legs.Count < 2)
    {
      throw new InvalidOperationException("Phải có ít nhất 2 chặng bay cho tìm kiếm nhiều chặng.");
    }

    if (request.Legs.Count > 6)
    {
      throw new InvalidOperationException("Tối đa 6 chặng bay cho mỗi lần tìm kiếm.");
    }

    var vietnamToday = DateTime.UtcNow.AddHours(7).Date;

    for (int i = 0; i < request.Legs.Count; i++)
    {
      var leg = request.Legs[i];

      if (string.IsNullOrWhiteSpace(leg.DepartureAirport) || string.IsNullOrWhiteSpace(leg.ArrivalAirport))
      {
        throw new InvalidOperationException($"Chặng {i + 1}: Thông tin sân bay không hợp lệ.");
      }

      if (leg.DepartureAirport.Trim().Equals(leg.ArrivalAirport.Trim(), StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Chặng {i + 1}: Điểm đi phải khác điểm đến.");
      }

      if (leg.DepartureDate.Date < vietnamToday)
      {
        throw new InvalidOperationException($"Chặng {i + 1}: Ngày bay không được nhỏ hơn ngày hiện tại.");
      }

      if (!AppConstants.ValidSeatClasses.Contains(leg.SeatClass.Trim(), StringComparer.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Chặng {i + 1}: Hạng ghế không hợp lệ.");
      }

      if (i > 0)
      {
        var prevLeg = request.Legs[i - 1];
        if (leg.DepartureDate.Date < prevLeg.DepartureDate.Date)
        {
          throw new InvalidOperationException($"Chặng {i + 1}: Ngày bay không được trước ngày của chặng trước đó.");
        }
      }
    }

    if (request.PassengerCount < 1 || request.PassengerCount > 9)
    {
      throw new InvalidOperationException("Số lượng hành khách phải từ 1 đến 9.");
    }

    if (request.InfantCount > request.PassengerCount)
    {
      throw new InvalidOperationException("Số em bé không được vượt quá tổng số hành khách.");
    }

    var seenLegs = new HashSet<string>();
    foreach (var leg in request.Legs)
    {
      var key = $"{leg.DepartureAirport.Trim().ToLower()}|{leg.ArrivalAirport.Trim().ToLower()}|{leg.DepartureDate:yyyy-MM-dd}";
      if (!seenLegs.Add(key))
      {
        throw new InvalidOperationException("Không được chọn trùng lặp (điểm đi, điểm đến, ngày) giữa các chặng.");
      }
    }
  }

  private static void ValidateSearchRequest(FlightSearchRequestDto request)
    {
        if (request.DepartureAirport.Trim().Equals(request.ArrivalAirport.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Điểm đi phải khác điểm đến.");
        }

        var vietnamToday = DateTime.UtcNow.AddHours(7).Date;
        if (request.DepartureDate.Date < vietnamToday)
        {
            throw new InvalidOperationException("Ngày bay không được nhỏ hơn ngày hiện tại.");
        }

        if (request.PassengerCount < 1 || request.PassengerCount > 9)
        {
            throw new InvalidOperationException("Số lượng hành khách phải từ 1 đến 9.");
        }

        if (request.ReturnDate.HasValue && request.ReturnDate.Value.Date < request.DepartureDate.Date)
        {
            throw new InvalidOperationException("Ngày về không được nhỏ hơn ngày đi.");
        }

        if (!AppConstants.ValidSeatClasses.Contains(request.SeatClass.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Hạng ghế không hợp lệ.");
        }
    }

    public async Task<List<FlightStatusListItemDto>> GetStaffFlightListAsync(DateTime? date, string? departureAirport, string? arrivalAirport, string? status)
    {
        IQueryable<Models.Entities.Flight> q = _dbContext.Flights
            .AsNoTracking();

        if (date.HasValue)
        {
            var (utcStart, utcEnd) = GetUtcDayRange(date.Value.Date);
            q = q.Where(f => f.DepartureTime >= utcStart && f.DepartureTime < utcEnd);
        }
        else
        {
            var vietnamToday = DateTime.UtcNow.AddHours(7).Date;
            var (windowStart, _) = GetUtcDayRange(vietnamToday.AddDays(-60));
            var (_, windowEnd) = GetUtcDayRange(vietnamToday.AddDays(60));
            q = q.Where(f => f.DepartureTime >= windowStart && f.DepartureTime < windowEnd);
        }

        var depNorm = string.IsNullOrWhiteSpace(departureAirport) ? "" : NormalizeAirportName(departureAirport);
        var arrNorm = string.IsNullOrWhiteSpace(arrivalAirport) ? "" : NormalizeAirportName(arrivalAirport);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim();
            q = q.Where(f => f.Status.Contains(s));
        }

        // Load flights - airport filtering is done in-memory due to normalization
        var allFlights = await q.OrderBy(f => f.DepartureTime).ToListAsync();

        var flights = allFlights
            .Where(f => string.IsNullOrWhiteSpace(depNorm) || NormalizeAirportName(f.DepartureAirport) == depNorm)
            .Where(f => string.IsNullOrWhiteSpace(arrNorm) || NormalizeAirportName(f.ArrivalAirport) == arrNorm)
            .ToList();
        if (flights.Count == 0) return new List<FlightStatusListItemDto>();

        var flightIds = flights.Select(f => f.Id).ToList();
        var nowUtc = DateTime.UtcNow;

        var ticketStats = await _dbContext.Tickets
            .Where(t => flightIds.Contains(t.FlightId)
                && t.TicketStatus != AppConstants.CancelledStatus
                && t.TicketStatus != AppConstants.NoShowStatus
                && !(t.Booking != null
                     && t.Booking.BookingStatus == AppConstants.PendingPaymentStatus
                     && t.Booking.ExpiresAt < nowUtc))
            .GroupBy(t => new { t.FlightId, t.SeatClass })
            .Select(g => new
            {
                g.Key.FlightId,
                g.Key.SeatClass,
                BookedTickets = g.Count(),
                CheckedIn = g.Count(t => t.IsCheckedIn)
            })
            .ToListAsync();

        var seatCounts = await _dbContext.Seats
            .Where(s => flightIds.Contains(s.FlightId))
            .GroupBy(s => new { s.FlightId, s.SeatClass })
            .Select(g => new { g.Key.FlightId, g.Key.SeatClass, Count = g.Count() })
            .ToListAsync();

        return flights.Select(f =>
        {
            var econTotal = seatCounts.FirstOrDefault(s => s.FlightId == f.Id && s.SeatClass == AppConstants.EconomySeatClass)?.Count ?? 0;
            var bizTotal = seatCounts.FirstOrDefault(s => s.FlightId == f.Id && s.SeatClass == AppConstants.BusinessSeatClass)?.Count ?? 0;
            var econMax = AppConstants.CalcMaxAllowedTickets(econTotal, _overbookingRatio);
            var bizMax = AppConstants.CalcMaxAllowedTickets(bizTotal, _overbookingRatio);

            var econStat = ticketStats.FirstOrDefault(x => x.FlightId == f.Id && x.SeatClass == AppConstants.EconomySeatClass);
            var bizStat = ticketStats.FirstOrDefault(x => x.FlightId == f.Id && x.SeatClass == AppConstants.BusinessSeatClass);

            var econBooked = econStat?.BookedTickets ?? 0;
            var bizBooked = bizStat?.BookedTickets ?? 0;

            var oversold = Math.Max(0, econBooked - econTotal) + Math.Max(0, bizBooked - bizTotal);

            return new FlightStatusListItemDto
            {
                FlightId = f.Id,
                FlightNumber = f.FlightNumber,
                DepartureAirport = f.DepartureAirport,
                ArrivalAirport = f.ArrivalAirport,
                DepartureTime = f.DepartureTime,
                ArrivalTime = f.ArrivalTime,
                Status = f.Status,
                EconomyTotalSeats = econTotal,
                EconomyMaxAllowedTickets = econMax,
                EconomyBookedTickets = econBooked,
                EconomyCheckedIn = econStat?.CheckedIn ?? 0,
                BusinessTotalSeats = bizTotal,
                BusinessMaxAllowedTickets = bizMax,
                BusinessBookedTickets = bizBooked,
                BusinessCheckedIn = bizStat?.CheckedIn ?? 0,
                OversoldTickets = oversold,
                OverbookingRatio = _overbookingRatio
            };
        }).ToList();
    }

    public async Task<List<OversellPassengerDto>> GetOversellPassengersAsync(int flightId)
    {
        var flight = await _dbContext.Flights
            .Include(f => f.Seats)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == flightId);

        if (flight is null)
        {
            throw new InvalidOperationException("Không tìm thấy chuyến bay.");
        }

        var economyTotal = flight.Seats.Count(s => s.SeatClass == AppConstants.EconomySeatClass);
        var businessTotal = flight.Seats.Count(s => s.SeatClass == AppConstants.BusinessSeatClass);

        var tickets = await _dbContext.Tickets
            .Include(t => t.Booking)
            .Include(t => t.Passenger)
            .Where(t => t.FlightId == flightId
                && t.TicketStatus != AppConstants.CancelledStatus
                && t.TicketStatus != AppConstants.NoShowStatus)
            .OrderBy(t => t.IsCheckedIn)
            .ThenByDescending(t => t.Booking!.CreatedAt)
            .ToListAsync();

        var result = new List<OversellPassengerDto>();

        result.AddRange(tickets
            .Where(t => t.SeatClass == AppConstants.EconomySeatClass)
            .Skip(economyTotal)
            .Select(ToOversellPassengerDto));

        result.AddRange(tickets
            .Where(t => t.SeatClass == AppConstants.BusinessSeatClass)
            .Skip(businessTotal)
            .Select(ToOversellPassengerDto));

        return result;
    }

    public async Task<string> HandleOversellAsync(int flightId, HandleOversellRequestDto request)
    {
        var action = (request.Action ?? string.Empty).Trim();
        if (!string.Equals(action, AppConstants.OversellActionVoluntaryBump, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(action, AppConstants.OversellActionNoShow, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Action không hợp lệ. Hợp lệ: VoluntaryBump, NoShow.");
        }

        var ticket = await _dbContext.Tickets
            .Include(t => t.Booking)
            .Include(t => t.Passenger)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.FlightId == flightId);

        if (ticket is null || ticket.Booking is null)
        {
            throw new InvalidOperationException("Không tìm thấy vé cần xử lý.");
        }

        if (ticket.IsCheckedIn && string.Equals(action, AppConstants.OversellActionVoluntaryBump, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Không thể bump khách đã check-in. Chỉ có thể đánh dấu No-show nếu khách không đến.");
        }

        if (string.Equals(action, AppConstants.OversellActionNoShow, StringComparison.OrdinalIgnoreCase))
        {
            ticket.TicketStatus = AppConstants.NoShowStatus;
            await _dbContext.SaveChangesAsync();
            return $"Đã đánh dấu No-show cho vé #{ticket.Id} ({ticket.Passenger?.FullName}). Chỗ này được giải phóng khỏi danh sách vé tồn.";
        }

        ticket.TicketStatus = AppConstants.CancelledStatus;
        ticket.AllowCancellation = false;

        var couponCode = $"BUMP{flightId}{ticket.Id}{DateTime.UtcNow:HHmm}";
        _dbContext.Coupons.Add(new Models.Entities.Coupon
        {
            Code = couponCode,
            DiscountPercent = 20,
            MaxUses = 1,
            CurrentUses = 0,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return $"Đã xử lý Voluntary Bump cho vé #{ticket.Id}. Tạo voucher {couponCode} giảm 20% cho khách tự nguyện đổi chuyến.";
    }

    private static OversellPassengerDto ToOversellPassengerDto(Models.Entities.Ticket t)
    {
        return new OversellPassengerDto
        {
            TicketId = t.Id,
            BookingId = t.BookingId,
            PnrCode = t.Booking?.PnrCode ?? string.Empty,
            PassengerName = t.Passenger?.FullName ?? string.Empty,
            SeatClass = t.SeatClass,
            TicketStatus = t.TicketStatus,
            IsCheckedIn = t.IsCheckedIn,
            Price = t.Price,
            BookingCreatedAt = t.Booking?.CreatedAt ?? DateTime.MinValue,
            ContactEmail = t.Booking?.ContactEmail ?? string.Empty,
            ContactPhoneNumber = t.Booking?.ContactPhoneNumber ?? string.Empty
        };
    }
}
