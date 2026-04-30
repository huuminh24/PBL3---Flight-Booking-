using AirlineBookingApi.Configurations;
using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.Flights;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AirlineBookingApi.Services.Implementations;

public class FlightService : IFlightService
{
    private readonly AppDbContext _dbContext;
    private readonly decimal _overbookingRatio;

    public FlightService(AppDbContext dbContext, IOptions<BookingSettings> bookingOptions)
    {
        _dbContext = dbContext;
        _overbookingRatio = bookingOptions?.Value?.OverbookingRatio > 0
            ? bookingOptions.Value.OverbookingRatio
            : AppConstants.DefaultOverbookingRatio;
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

        var normalizedDepartureAirport = request.DepartureAirport.Trim().ToLower();
        var normalizedArrivalAirport = request.ArrivalAirport.Trim().ToLower();
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
  depTimeFrom,
  depTimeTo,
  airlines);

  result.ReturnFlights = returnFlights;
}

        return result;
    }

private async Task<List<FlightSearchResponseDto>> SearchSingleDirectionAsync(string depAirport, string arrAirport, DateTime date, string seatClass, int paxCount, int infantCount = 0, int? depTimeFrom = null, int? depTimeTo = null, List<string>? airlines = null)
{
  var query = _dbContext.Flights
  .Include(x => x.FlightPrices)
  .Include(x => x.Seats)
  .Include(x => x.Airline)
  .Where(x => x.DepartureAirport.ToLower() == depAirport
  && x.ArrivalAirport.ToLower() == arrAirport
  && x.DepartureTime.Date == date);

  if (depTimeFrom.HasValue)
  {
    query = query.Where(x => x.DepartureTime.Hour >= depTimeFrom.Value);
  }

  if (depTimeTo.HasValue)
  {
    query = query.Where(x => x.DepartureTime.Hour < depTimeTo.Value);
  }

  if (airlines != null && airlines.Count > 0)
  {
    query = query.Where(x => x.Airline != null && airlines.Contains(x.Airline.Code.ToLower()));
  }

  var flights = await query.ToListAsync();

  var flightIds = flights.Select(f => f.Id).ToList();

  var nowUtc = DateTime.UtcNow;
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

    if (selectedPrice is null || availableSeatCount < seatsNeeded)
    {
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
    var seatClass = legs[0].SeatClass.Trim();
    var adultCount = request.PassengerCount - request.InfantCount;

    var legResults = new List<List<FlightSearchResponseDto>>();

    for (int i = 0; i < legs.Count; i++)
    {
      var leg = legs[i];
      var normalizedDep = leg.DepartureAirport.Trim();
      var normalizedArr = leg.ArrivalAirport.Trim();

      var results = await SearchSingleDirectionAsync(
        normalizedDep,
        normalizedArr,
        leg.DepartureDate.Date,
        seatClass,
        adultCount,
        request.InfantCount,
        request.DepartureTimeFrom,
        request.DepartureTimeTo,
        request.Airlines);

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

      if (leg.DepartureDate.Date < DateTime.Today)
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

        if (request.DepartureDate.Date < DateTime.Today)
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
            .Include(f => f.Seats)
            .AsNoTracking();

        if (date.HasValue)
        {
            // So sánh theo .Date của DepartureTime (đã được normalize UTC bởi value converter).
            var dayUtc = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var nextDayUtc = dayUtc.AddDays(1);
            q = q.Where(f => f.DepartureTime >= dayUtc && f.DepartureTime < nextDayUtc);
        }
        else
        {
            // Mặc định: lấy chuyến trong khoảng ±60 ngày so với hiện tại để Staff thấy
            // các chuyến vừa khai thác và sắp khởi hành. Tránh load toàn bộ DB.
            var nowDate = DateTime.UtcNow.Date;
            var windowStart = DateTime.SpecifyKind(nowDate.AddDays(-60), DateTimeKind.Utc);
            var windowEnd = DateTime.SpecifyKind(nowDate.AddDays(60), DateTimeKind.Utc);
            q = q.Where(f => f.DepartureTime >= windowStart && f.DepartureTime < windowEnd);
        }

        if (!string.IsNullOrWhiteSpace(departureAirport))
        {
            var dep = departureAirport.Trim();
            q = q.Where(f => f.DepartureAirport.Contains(dep));
        }

        if (!string.IsNullOrWhiteSpace(arrivalAirport))
        {
            var arr = arrivalAirport.Trim();
            q = q.Where(f => f.ArrivalAirport.Contains(arr));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim();
            q = q.Where(f => f.Status.Contains(s));
        }

        var flights = await q.OrderBy(f => f.DepartureTime).ToListAsync();
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

        return flights.Select(f =>
        {
            var econTotal = f.Seats.Count(s => s.SeatClass == AppConstants.EconomySeatClass);
            var bizTotal = f.Seats.Count(s => s.SeatClass == AppConstants.BusinessSeatClass);
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
