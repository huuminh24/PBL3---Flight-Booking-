using AirlineBookingApi.Configurations;
using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.StaffBookings;
using AirlineBookingApi.Models.Entities;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace AirlineBookingApi.Services.Implementations;

public class StaffBookingService : IStaffBookingService
{
  private readonly AppDbContext _dbContext;
  private readonly decimal _overbookingRatio;

  public StaffBookingService(AppDbContext dbContext, IOptions<BookingSettings> bookingOptions)
  {
    _dbContext = dbContext;
    _overbookingRatio = bookingOptions?.Value?.OverbookingRatio > 0
      ? bookingOptions.Value.OverbookingRatio
      : AppConstants.DefaultOverbookingRatio;
  }

  public async Task<CreateStaffBookingResponseDto> CreateStaffBookingAsync(CreateStaffBookingRequestDto request, int currentStaffId, string currentRole)
  {
    if (!string.Equals(currentRole, AppConstants.StaffRoleName, StringComparison.OrdinalIgnoreCase))
    {
      throw new InvalidOperationException("Chỉ Staff mới được phép tạo booking hộ khách hàng.");
    }

    await using var tx = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

    ValidateRequest(request);

    int? customerAccountId = null;
    if (request.CustomerAccountId.HasValue)
    {
      var customerAccount = await _dbContext.Accounts
      .Include(x => x.Role)
      .FirstOrDefaultAsync(x => x.Id == request.CustomerAccountId.Value);

      if (customerAccount is null)
      {
        throw new InvalidOperationException("Tài khoản khách hàng không tồn tại.");
      }

      if (!string.Equals(customerAccount.Role?.Name, AppConstants.CustomerRoleName, StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException("Tài khoản được chọn không phải Customer.");
      }

      customerAccountId = customerAccount.Id;
    }

    var booking = new Booking
    {
      PnrCode = await GeneratePnrCodeAsync(),
      BookingStatus = AppConstants.PendingPaymentStatus,
      CustomerAccountId = customerAccountId,
      CreatedByAccountId = currentStaffId,
      ContactFullName = request.ContactFullName.Trim(),
      ContactEmail = request.ContactEmail.Trim(),
      ContactPhoneNumber = request.ContactPhoneNumber.Trim(),
      TotalAmount = 0,
      BookingChannel = AppConstants.StaffChannel,
      ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.BookingPaymentExpiryMinutes)
    };

    var passengers = new List<Passenger>();
    foreach (var passengerRequest in request.Passengers)
    {
      passengers.Add(new Passenger
      {
        FullName = passengerRequest.FullName.Trim(),
        DateOfBirth = passengerRequest.DateOfBirth,
        Gender = passengerRequest.Gender.Trim(),
        PassengerType = passengerRequest.PassengerType.Trim()
      });
    }

    decimal totalAmount = 0;
    int requiredSeats = request.Passengers.Count(x => !string.Equals(x.PassengerType, AppConstants.InfantPassengerType, StringComparison.OrdinalIgnoreCase));
    int legOrder = 1;

    foreach (var passenger in passengers)
    {
      booking.Passengers.Add(passenger);
    }

    foreach (var flightRequest in request.Flights)
    {
      var flight = await _dbContext.Flights
      .Include(x => x.Seats)
      .Include(x => x.FlightPrices)
      .FirstOrDefaultAsync(x => x.Id == flightRequest.FlightId);

      if (flight is null)
      {
        throw new InvalidOperationException($"Chuyến bay ID {flightRequest.FlightId} không tồn tại.");
      }

      var normalizedSeatClass = flightRequest.SeatClass.Trim();
      var selectedPrice = flight.FlightPrices.FirstOrDefault(x => x.SeatClass == normalizedSeatClass);
      if (selectedPrice is null)
      {
        throw new InvalidOperationException($"Hạng vé {normalizedSeatClass} không tồn tại cho chuyến bay {flight.FlightNumber}.");
      }

      var totalSeatsInClass = flight.Seats.Count(s => s.SeatClass == normalizedSeatClass);
      var maxAllowedTickets = AppConstants.CalcMaxAllowedTickets(totalSeatsInClass, _overbookingRatio);
      var nowUtc = DateTime.UtcNow;
      var bookedTicketsInClass = await _dbContext.Tickets
      .CountAsync(t => t.FlightId == flight.Id
          && t.SeatClass == normalizedSeatClass
          && t.TicketStatus != AppConstants.CancelledStatus
          && !(t.Booking != null
               && t.Booking.BookingStatus == AppConstants.PendingPaymentStatus
               && t.Booking.ExpiresAt < nowUtc));
      var availableSeatCount = maxAllowedTickets - bookedTicketsInClass;

      if (availableSeatCount < requiredSeats)
      {
        throw new InvalidOperationException($"Không đủ chỗ cho hạng {normalizedSeatClass} trên chuyến bay {flight.FlightNumber}. Còn {Math.Max(0, availableSeatCount)} chỗ (đã tính oversell {(_overbookingRatio - 1m) * 100m:0.#}%), cần {requiredSeats} chỗ.");
      }

      decimal flightTotal = 0;

      var bookingFlight = new BookingFlight
      {
        FlightId = flight.Id,
        LegOrder = legOrder,
        SeatClass = normalizedSeatClass
      };

      foreach (var passenger in passengers)
      {
        decimal ticketPrice = selectedPrice.Price;

        if (string.Equals(passenger.PassengerType, AppConstants.ChildPassengerType, StringComparison.OrdinalIgnoreCase))
        {
          ticketPrice = selectedPrice.Price * AppConstants.ChildPriceMultiplier;
        }
        else if (string.Equals(passenger.PassengerType, AppConstants.InfantPassengerType, StringComparison.OrdinalIgnoreCase))
        {
          ticketPrice = selectedPrice.Price * AppConstants.InfantPriceMultiplier;
        }

        flightTotal += ticketPrice;

        var ticket = new Ticket
        {
          FlightId = flight.Id,
          BookingFlight = bookingFlight,
          SeatClass = normalizedSeatClass,
          Price = ticketPrice,
          Passenger = passenger,
          TicketStatus = AppConstants.PendingPaymentStatus,
          AllowCancellation = true
        };

        booking.Tickets.Add(ticket);
        passenger.Tickets.Add(ticket);
      }

      bookingFlight.Price = flightTotal;
      booking.BookingFlights.Add(bookingFlight);
      totalAmount += flightTotal;
      legOrder++; // increment after processing this leg
    }

    booking.TotalAmount = totalAmount;

    _dbContext.Bookings.Add(booking);
    await _dbContext.SaveChangesAsync();

    await tx.CommitAsync();

    return MapBookingToResponse(booking, currentStaffId);
  }

  public async Task<CreateStaffBookingResponseDto?> GetStaffBookingByIdAsync(int bookingId)
  {
    var booking = await _dbContext.Bookings
    .Include(x => x.Passengers)
    .FirstOrDefaultAsync(x => x.Id == bookingId && x.BookingChannel == AppConstants.StaffChannel);

    if (booking is null)
    {
      return null;
    }

    return MapBookingToResponse(booking, booking.CreatedByAccountId);
  }

  public async Task<List<CreateStaffBookingResponseDto>> SearchStaffBookingsByPnrAsync(string pnr)
  {
    var bookings = await _dbContext.Bookings
    .Where(x => x.BookingChannel == AppConstants.StaffChannel && x.PnrCode.Contains(pnr))
    .Include(x => x.Passengers)
    .ToListAsync();

    return bookings.Select(x => MapBookingToResponse(x, x.CreatedByAccountId)).ToList();
  }

  private static readonly string[] ValidGenders = AppConstants.ValidGenders;
  private static readonly string[] ValidPassengerTypes = AppConstants.ValidPassengerTypes;

  private static void ValidateRequest(CreateStaffBookingRequestDto request)
  {
    if (request.Flights == null || request.Flights.Count == 0)
    {
      throw new InvalidOperationException("Phải có ít nhất một chặng bay.");
    }

    if (request.Flights.GroupBy(f => f.FlightId).Any(g => g.Count() > 1))
    {
      throw new InvalidOperationException("Không được chọn cùng một chuyến bay hai lần trong một booking.");
    }

    foreach (var flight in request.Flights)
    {
      if (!AppConstants.ValidSeatClasses.Contains(flight.SeatClass.Trim(), StringComparer.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException("Hạng ghế không hợp lệ.");
      }
    }

    if (request.Passengers.Count is < 1 or > 9)
    {
      throw new InvalidOperationException("Số lượng hành khách phải từ 1 đến 9.");
    }

    foreach (var pax in request.Passengers)
    {
      if (string.IsNullOrWhiteSpace(pax.FullName))
      {
        throw new InvalidOperationException("Thông tin hành khách không hợp lệ.");
      }

      if (!ValidGenders.Contains(pax.Gender.Trim(), StringComparer.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Giới tính '{pax.Gender}' không hợp lệ. Chỉ chấp nhận: Male, Female.");
      }

      if (!ValidPassengerTypes.Contains(pax.PassengerType.Trim(), StringComparer.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Loại hành khách '{pax.PassengerType}' không hợp lệ. Chỉ chấp nhận: Adult, Child, Infant.");
      }
    }
  }

  private async Task<string> GeneratePnrCodeAsync()
  {
    const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    var random = new Random();
    const int maxAttempts = 10;

    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
      var suffix = new string(Enumerable.Range(0, 4)
      .Select(_ => chars[random.Next(chars.Length)])
      .ToArray());

      var pnrCode = $"BK{DateTime.UtcNow:yy}{suffix}";
      var exists = await _dbContext.Bookings.AnyAsync(x => x.PnrCode == pnrCode);

      if (!exists)
      {
        return pnrCode;
      }
    }

    throw new InvalidOperationException("Không sinh được mã PNR duy nhất, vui lòng thử lại.");
  }

  private static CreateStaffBookingResponseDto MapBookingToResponse(Booking booking, int createdByStaffId)
  {
    return new CreateStaffBookingResponseDto
    {
      BookingId = booking.Id,
      PnrCode = booking.PnrCode,
      BookingStatus = booking.BookingStatus,
      BookingChannel = booking.BookingChannel,
      ContactFullName = booking.ContactFullName,
      TotalAmount = booking.TotalAmount,
      PassengerCount = booking.Passengers.Count,
      CreatedByStaffId = createdByStaffId,
      CustomerAccountId = booking.CustomerAccountId,
      ExpiresAt = booking.ExpiresAt
    };
  }
}