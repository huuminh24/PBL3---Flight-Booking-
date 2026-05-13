using AirlineBookingApi.Data;
using AirlineBookingApi.Constants;
using AirlineBookingApi.Models.DTOs.Payments;
using AirlineBookingApi.Models.Entities;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AirlineBookingApi.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _dbContext;
    private readonly ICouponService _couponService;

    public PaymentService(AppDbContext dbContext, ICouponService couponService)
    {
        _dbContext = dbContext;
        _couponService = couponService;
    }

    public async Task<CreatePaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request, int currentAccountId, string currentRole)
    {
        ValidateRequest(request);

        await using var tx = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var booking = await FindBookingAsync(request);
        if (booking is null)
        {
            throw new InvalidOperationException("Booking không tồn tại.");
        }

        if (currentRole == AppConstants.CustomerRoleName)
        {
            if (booking.CustomerAccountId != currentAccountId)
            {
                throw new InvalidOperationException("Customer chỉ được thanh toán booking của chính mình.");
            }
        }
        else if (currentRole == AppConstants.StaffRoleName)
        {
            // Staff có thể thanh toán bất kỳ booking nào
        }
        else
        {
            throw new InvalidOperationException("Role hiện tại không được phép thanh toán.");
        }

        if (!string.Equals(booking.BookingStatus, AppConstants.PendingPaymentStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Booking không ở trạng thái chờ thanh toán.");
        }

        var tickets = await _dbContext.Tickets
            .Where(x => x.BookingId == booking.Id)
            .ToListAsync();

        // Booking đã quá hạn thanh toán -> tự huỷ và từ chối.
        if (DateTime.UtcNow > booking.ExpiresAt)
        {
            booking.BookingStatus = AppConstants.CancelledStatus;
            foreach (var ticket in tickets)
            {
                if (ticket.TicketStatus != AppConstants.CancelledStatus)
                {
                    ticket.TicketStatus = AppConstants.CancelledStatus;
                }
            }
            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            throw new InvalidOperationException("Booking đã hết hạn thanh toán, vui lòng đặt lại.");
        }

        // Quy tắc nghiệp vụ: Cash chỉ dành cho Staff thao tác trên booking kên StaffChannel.
        var trimmedMethod = request.PaymentMethod.Trim();
        if (string.Equals(trimmedMethod, AppConstants.CashPaymentMethod, StringComparison.OrdinalIgnoreCase))
        {
            if (!string.Equals(currentRole, AppConstants.StaffRoleName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Khách đặt online không được thanh toán tiền mặt.");
            }
            if (!string.Equals(booking.BookingChannel, AppConstants.StaffChannel, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Chỉ booking tạo qua quầy (Staff) mới được thanh toán tiền mặt.");
            }
        }

        decimal discountAmount = 0;
        string? appliedCouponCode = null;
        decimal paymentAmount = booking.TotalAmount;

        // Apply coupon if provided
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var couponResult = await _couponService.ValidateCouponAsync(request.CouponCode.Trim());
            if (couponResult.IsValid)
            {
                discountAmount = booking.TotalAmount * couponResult.DiscountPercent / 100;
                paymentAmount = booking.TotalAmount - discountAmount;
                appliedCouponCode = couponResult.Code;
            }
            else
            {
                throw new InvalidOperationException($"Mã giảm giá không hợp lệ: {couponResult.Message}");
            }
        }

        var paymentStatus = request.IsSuccess ? AppConstants.PaidStatus : AppConstants.FailedStatus;

        var paymentReference = ResolvePaymentReference(trimmedMethod, request);

        var payment = new Payment
        {
            BookingId = booking.Id,
            PaymentMethod = trimmedMethod,
            Amount = paymentAmount,
            DiscountAmount = discountAmount,
            CouponCode = appliedCouponCode,
            PaymentStatus = paymentStatus,
            PaidAt = DateTime.UtcNow,
            PaymentReference = paymentReference
        };

        if (request.IsSuccess)
        {
            booking.BookingStatus = AppConstants.PaidStatus;

            foreach (var ticket in tickets)
            {
                ticket.TicketStatus = AppConstants.PaidStatus;
            }

            // Increment coupon usage after successful payment
            if (!string.IsNullOrWhiteSpace(appliedCouponCode))
            {
                await _couponService.UseCouponAsync(appliedCouponCode);
            }
        }

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();

        await tx.CommitAsync();

        return new CreatePaymentResponseDto
        {
            PaymentId = payment.Id,
            BookingId = booking.Id,
            PnrCode = booking.PnrCode,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = payment.PaymentStatus,
            BookingStatus = booking.BookingStatus,
            OriginalAmount = booking.TotalAmount,
            DiscountAmount = discountAmount,
            Amount = payment.Amount,
            CouponCode = appliedCouponCode,
            PaidAt = payment.PaidAt,
            PaymentReference = payment.PaymentReference
        };
    }

    private static string? ResolvePaymentReference(string method, CreatePaymentRequestDto request)
    {
        if (string.Equals(method, AppConstants.CardPaymentMethod, StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(request.CardLast4) ? null : $"CARD****{request.CardLast4.Trim()}";
        }

        if (string.Equals(method, AppConstants.BankTransferPaymentMethod, StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(request.BankAccountLast4) ? null : $"BANK****{request.BankAccountLast4.Trim()}";
        }

        if (string.Equals(method, AppConstants.VietQRPaymentMethod, StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(request.QrTransactionId) ? null : request.QrTransactionId.Trim();
        }

        return null;
    }

    private async Task<Booking?> FindBookingAsync(CreatePaymentRequestDto request)
    {
        if (request.BookingId.HasValue)
        {
            return await _dbContext.Bookings.FirstOrDefaultAsync(x => x.Id == request.BookingId.Value);
        }

        return await _dbContext.Bookings.FirstOrDefaultAsync(x => x.PnrCode == request.PnrCode);
    }

    private static void ValidateRequest(CreatePaymentRequestDto request)
    {
        var hasBookingId = request.BookingId.HasValue;
        var hasPnr = !string.IsNullOrWhiteSpace(request.PnrCode);

        if (!hasBookingId && !hasPnr)
        {
            throw new InvalidOperationException("Cần cung cấp BookingId hoặc PnrCode để thanh toán.");
        }

        if (!AppConstants.ValidPaymentMethods.Contains(request.PaymentMethod.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Phương thức thanh toán không hợp lệ.");
        }
    }
}
