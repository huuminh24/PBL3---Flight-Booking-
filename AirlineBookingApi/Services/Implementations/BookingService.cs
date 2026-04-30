using AirlineBookingApi.Configurations;
using AirlineBookingApi.Data;
using AirlineBookingApi.Constants;
using AirlineBookingApi.Models.DTOs.Bookings;
using AirlineBookingApi.Models.Entities;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace AirlineBookingApi.Services.Implementations;

public class BookingService : IBookingService
{
    private static readonly string[] ValidSeatClasses = AppConstants.ValidSeatClasses;

    private readonly AppDbContext _dbContext;
    private readonly decimal _overbookingRatio;

    public BookingService(AppDbContext dbContext, IOptions<BookingSettings> bookingOptions)
    {
        _dbContext = dbContext;
        _overbookingRatio = bookingOptions?.Value?.OverbookingRatio > 0
            ? bookingOptions.Value.OverbookingRatio
            : AppConstants.DefaultOverbookingRatio;
    }

    public async Task<CreateBookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request, int currentAccountId, string currentRole)
    {
        ValidateRequest(request);

        await using var tx = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        int? customerAccountId = null;

        if (currentRole == AppConstants.CustomerRoleName)
        {
            customerAccountId = currentAccountId;

            if (request.CustomerAccountId.HasValue && request.CustomerAccountId.Value != currentAccountId)
            {
                throw new InvalidOperationException("Customer chỉ được tạo booking cho chính mình.");
            }
        }
        else if (currentRole == AppConstants.StaffRoleName)
        {
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
        }
        else
        {
            throw new InvalidOperationException("Role hiện tại không được phép tạo booking.");
        }

        // Dọn các booking PendingPayment đã quá hạn để ghế được giải phóng trước khi check availability.
        await ReleaseExpiredBookingsInternalAsync();

        var booking = new Booking
        {
            PnrCode = await GeneratePnrCodeAsync(),
            BookingStatus = AppConstants.PendingPaymentStatus,
            CustomerAccountId = customerAccountId,
            CreatedByAccountId = currentAccountId,
            ContactFullName = request.ContactFullName.Trim(),
            ContactEmail = request.ContactEmail.Trim(),
            ContactPhoneNumber = request.ContactPhoneNumber.Trim(),
            TotalAmount = 0, 
            BookingChannel = currentRole == AppConstants.StaffRoleName ? AppConstants.StaffChannel : AppConstants.WebsiteChannel,
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
            // Chỉ tính các vé còn "chiếm chỗ": loại Cancelled và loại các booking PendingPayment đã quá ExpiresAt.
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

        return new CreateBookingResponseDto
        {
            BookingId = booking.Id,
            PnrCode = booking.PnrCode,
            BookingStatus = booking.BookingStatus,
            BookingChannel = booking.BookingChannel,
            ContactFullName = booking.ContactFullName,
            TotalAmount = booking.TotalAmount,
            PassengerCount = booking.Passengers.Count,
            ExpiresAt = booking.ExpiresAt
        };
    }

    public async Task<int> ReleaseExpiredBookingsAsync()
    {
        return await ReleaseExpiredBookingsInternalAsync();
    }

    private async Task<int> ReleaseExpiredBookingsInternalAsync()
    {
        var nowUtc = DateTime.UtcNow;
        var expired = await _dbContext.Bookings
            .Include(b => b.Tickets)
                .ThenInclude(t => t.Seat)
            .Where(b => b.BookingStatus == AppConstants.PendingPaymentStatus && b.ExpiresAt < nowUtc)
            .ToListAsync();

        if (expired.Count == 0)
        {
            return 0;
        }

        foreach (var booking in expired)
        {
            booking.BookingStatus = AppConstants.CancelledStatus;
            foreach (var ticket in booking.Tickets)
            {
                if (ticket.TicketStatus != AppConstants.CancelledStatus)
                {
                    ticket.TicketStatus = AppConstants.CancelledStatus;
                }
                if (ticket.Seat is not null)
                {
                    ticket.Seat.IsAvailable = true;
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        return expired.Count;
    }

    public async Task<InvoiceDto?> GetInvoiceAsync(int bookingId, int currentAccountId, string currentRole)
    {
        var booking = await _dbContext.Bookings
            .Include(x => x.BookingFlights)
                .ThenInclude(bf => bf.Flight)
            .Include(x => x.Passengers)
            .Include(x => x.Tickets)
                .ThenInclude(t => t.Passenger)
            .Include(x => x.Tickets)
                .ThenInclude(t => t.Flight)
            .Include(x => x.Tickets)
                .ThenInclude(t => t.Seat)
            .FirstOrDefaultAsync(x => x.Id == bookingId);

        if (booking is null)
        {
            return null;
        }

        if (string.Equals(currentRole, AppConstants.CustomerRoleName, StringComparison.OrdinalIgnoreCase))
        {
            if (booking.CustomerAccountId != currentAccountId)
            {
                throw new InvalidOperationException("Khách hàng chỉ được xem hoá đơn booking của chính mình.");
            }
        }
        else if (!string.Equals(currentRole, AppConstants.StaffRoleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Role hiện tại không được phép xem hoá đơn.");
        }

        var payment = await _dbContext.Payments
            .Where(p => p.BookingId == booking.Id)
            .OrderByDescending(p => p.PaidAt)
            .FirstOrDefaultAsync();

        return new InvoiceDto
        {
            BookingId = booking.Id,
            PnrCode = booking.PnrCode,
            BookingStatus = booking.BookingStatus,
            BookingChannel = booking.BookingChannel,
            CreatedAt = booking.CreatedAt,
            ExpiresAt = booking.ExpiresAt,
            ContactFullName = booking.ContactFullName,
            ContactEmail = booking.ContactEmail,
            ContactPhoneNumber = booking.ContactPhoneNumber,
            TotalAmount = booking.TotalAmount,
            Flights = booking.BookingFlights
                .OrderBy(bf => bf.LegOrder)
                .Select(bf => new InvoiceFlightDto
                {
                    LegOrder = bf.LegOrder,
                    FlightNumber = bf.Flight?.FlightNumber ?? string.Empty,
                    DepartureAirport = bf.Flight?.DepartureAirport ?? string.Empty,
                    ArrivalAirport = bf.Flight?.ArrivalAirport ?? string.Empty,
                    DepartureTime = bf.Flight?.DepartureTime ?? default,
                    ArrivalTime = bf.Flight?.ArrivalTime ?? default,
                    SeatClass = bf.SeatClass,
                    Price = bf.Price
                })
                .ToList(),
            Passengers = booking.Passengers
                .Select(p => new InvoicePassengerDto
                {
                    PassengerId = p.Id,
                    FullName = p.FullName,
                    Gender = p.Gender,
                    PassengerType = p.PassengerType,
                    DateOfBirth = p.DateOfBirth
                })
                .ToList(),
            Tickets = booking.Tickets
                .Select(t => new InvoiceTicketDto
                {
                    TicketId = t.Id,
                    PassengerFullName = t.Passenger?.FullName ?? string.Empty,
                    FlightNumber = t.Flight?.FlightNumber ?? string.Empty,
                    SeatClass = t.SeatClass,
                    SeatNumber = t.Seat?.SeatNumber,
                    IsCheckedIn = t.IsCheckedIn,
                    TicketStatus = t.TicketStatus,
                    Price = t.Price
                })
                .ToList(),
            Payment = payment is null ? null : new InvoicePaymentDto
            {
                PaymentId = payment.Id,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                OriginalAmount = payment.Amount + payment.DiscountAmount,
                DiscountAmount = payment.DiscountAmount,
                Amount = payment.Amount,
                CouponCode = payment.CouponCode,
                PaymentReference = payment.PaymentReference,
                PaidAt = payment.PaidAt
            }
        };
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

    private static readonly string[] ValidGenders = AppConstants.ValidGenders;
    private static readonly string[] ValidPassengerTypes = AppConstants.ValidPassengerTypes;

private static void ValidateRequest(CreateBookingRequestDto request)
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
    throw new InvalidOperationException("Mỗi booking phải có từ 1 đến 9 hành khách.");
  }

  foreach (var pax in request.Passengers)
  {
    if (string.IsNullOrWhiteSpace(pax.FullName))
    {
      throw new InvalidOperationException("Họ tên hành khách không được để trống.");
    }

    if (!ValidGenders.Contains(pax.Gender.Trim(), StringComparer.OrdinalIgnoreCase))
    {
      throw new InvalidOperationException($"Giới tính '{pax.Gender}' không hợp lệ. Chỉ chấp nhận: Male, Female.");
    }

    if (!AppConstants.ValidPassengerTypes.Contains(pax.PassengerType.Trim(), StringComparer.OrdinalIgnoreCase))
    {
      throw new InvalidOperationException($"Loại hành khách '{pax.PassengerType}' không hợp lệ. Chỉ chấp nhận: Adult, Child, Infant.");
    }
  }
}
}
