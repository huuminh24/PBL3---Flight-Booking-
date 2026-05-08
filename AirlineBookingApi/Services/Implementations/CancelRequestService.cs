using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Models.DTOs.CancelRequests;
using AirlineBookingApi.Models.Entities;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Services.Implementations;

public class CancelRequestService : ICancelRequestService
{

    private readonly AppDbContext _dbContext;

    public CancelRequestService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CancelRequestResponseDto> CreateCancelRequestAsync(CreateCancelRequestDto request, int currentAccountId, string currentRole)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new InvalidOperationException("Lý do hủy vé không được để trống.");
        }

        var ticket = await _dbContext.Tickets
            .Include(x => x.Booking)
            .Include(x => x.Flight)
            .FirstOrDefaultAsync(x => x.Id == request.TicketId);

        if (ticket is null)
        {
            throw new InvalidOperationException("Vé không tồn tại.");
        }

        var booking = ticket.Booking!;
        var flight = ticket.Flight!;

        if (currentRole == AppConstants.CustomerRoleName)
        {
            if (booking.CustomerAccountId != currentAccountId)
            {
                throw new InvalidOperationException("Bạn không có quyền yêu cầu hủy vé của người khác.");
            }
        }
        else if (currentRole == AppConstants.StaffRoleName)
        {
            if (booking.CreatedByAccountId != currentAccountId && booking.CustomerAccountId != currentAccountId)
            {
                throw new InvalidOperationException("Staff không có quyền xử lý vé này.");
            }
        }
        else
        {
            throw new InvalidOperationException("Role hiện tại không được phép yêu cầu hủy vé.");
        }

        if (flight.DepartureTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Vé đã bay, không thể yêu cầu hủy.");
        }

        if (ticket.IsCheckedIn || string.Equals(ticket.TicketStatus, AppConstants.CheckedInStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Vé đã check-in, không thể yêu cầu hủy.");
        }

        if (!ticket.AllowCancellation)
        {
            throw new InvalidOperationException("Hạng vé hiện tại không cho phép hủy.");
        }

        var remainingTime = flight.DepartureTime - DateTime.UtcNow;
        var requiredHoursBeforeDeparture = string.Equals(ticket.SeatClass, AppConstants.BusinessSeatClass, StringComparison.OrdinalIgnoreCase)
            ? AppConstants.BusinessCancelHours
            : AppConstants.EconomyCancelHours;

        if (remainingTime.TotalHours < requiredHoursBeforeDeparture)
        {
            throw new InvalidOperationException($"Đã quá thời hạn hủy vé. Hạng {ticket.SeatClass} phải hủy trước ít nhất {requiredHoursBeforeDeparture} giờ.");
        }

        if (!string.Equals(ticket.TicketStatus, AppConstants.PaidStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chỉ vé đã thanh toán mới được gửi yêu cầu hủy.");
        }

        var existingPendingRequest = await _dbContext.CancelRequests
            .AnyAsync(x => x.TicketId == ticket.Id && x.CancelStatus == AppConstants.CancelPendingStatus);

        if (existingPendingRequest)
        {
            throw new InvalidOperationException("Vé này đã có yêu cầu hủy đang chờ xử lý.");
        }

        var refundRate = string.Equals(ticket.SeatClass, AppConstants.BusinessSeatClass, StringComparison.OrdinalIgnoreCase)
            ? AppConstants.BusinessRefundRate
            : AppConstants.EconomyRefundRate;

        var cancelRequest = new CancelRequest
        {
            TicketId = ticket.Id,
            RequestedByAccountId = currentAccountId,
            Reason = request.Reason.Trim(),
            CancelStatus = AppConstants.CancelPendingStatus,
            RefundAmount = ticket.Price * refundRate
        };

        ticket.TicketStatus = AppConstants.CancelRequestedTicketStatus;

        _dbContext.CancelRequests.Add(cancelRequest);
        await _dbContext.SaveChangesAsync();

        return new CancelRequestResponseDto
        {
            CancelRequestId = cancelRequest.Id,
            TicketId = ticket.Id,
            PnrCode = booking.PnrCode,
            CancelStatus = cancelRequest.CancelStatus,
            TicketStatus = ticket.TicketStatus,
            RefundAmount = cancelRequest.RefundAmount,
            Reason = cancelRequest.Reason,
            FlightNumber = flight.FlightNumber,
            DepartureAirport = flight.DepartureAirport,
            ArrivalAirport = flight.ArrivalAirport,
            DepartureTime = flight.DepartureTime,
            SeatClass = ticket.SeatClass,
            RequestedAt = cancelRequest.CreatedAt,
            Message = "Yêu cầu hủy vé đã được tạo thành công."
        };
    }

    public async Task<CancelRequestResponseDto> ProcessCancelRequestAsync(int cancelRequestId, ProcessCancelRequestDto request, int currentStaffId, string currentRole)
    {
        // Chỉ Staff mới được duyệt/từ chối
        if (!string.Equals(currentRole, AppConstants.StaffRoleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chỉ Staff mới được xử lý yêu cầu hủy vé.");
        }

        var cancelRequest = await _dbContext.CancelRequests
            .Include(x => x.Ticket)
                .ThenInclude(x => x!.Booking)
            .Include(x => x.Ticket)
                .ThenInclude(x => x!.Seat)
            .FirstOrDefaultAsync(x => x.Id == cancelRequestId);

        if (cancelRequest is null)
        {
            throw new InvalidOperationException("Yêu cầu hủy vé không tồn tại.");
        }

        if (!string.Equals(cancelRequest.CancelStatus, AppConstants.CancelPendingStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Yêu cầu hủy vé này đã được xử lý trước đó.");
        }

        var ticket = cancelRequest.Ticket!;
        var booking = ticket.Booking!;

        if (request.IsApproved)
        {
            // ── DUYỆT: Hủy vé + trả ghế ──
            cancelRequest.CancelStatus = AppConstants.CancelApprovedStatus;
            ticket.TicketStatus = AppConstants.CancelledStatus;
            cancelRequest.StaffNote = request.StaffNote?.Trim();

            // Trả ghế lại để người khác đặt
            if (ticket.Seat is not null)
            {
                ticket.Seat.IsAvailable = true;
            }

            // Check if all tickets in the booking are now cancelled
            var allTickets = await _dbContext.Tickets
                .Where(t => t.BookingId == booking.Id)
                .ToListAsync();

            if (allTickets.All(t => t.TicketStatus == AppConstants.CancelledStatus))
            {
                booking.BookingStatus = AppConstants.CancelledStatus;
            }
        }
        else
        {
            // ── TỪ CHỐI: Hoàn trạng thái vé về Paid ──
            if (string.IsNullOrWhiteSpace(request.StaffNote))
            {
                throw new InvalidOperationException("Cần ghi rõ lý do khi từ chối yêu cầu hủy.");
            }

            cancelRequest.CancelStatus = AppConstants.CancelRejectedStatus;
            ticket.TicketStatus = AppConstants.PaidStatus; // Hoàn trạng thái vé về Paid
            cancelRequest.RefundAmount = 0;   // Không hoàn tiền
            cancelRequest.StaffNote = request.StaffNote?.Trim();
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Yêu cầu hủy này vừa được đồng nghiệp xử lý. Vui lòng tải lại danh sách.");
        }

        var message = request.IsApproved
            ? $"Đã duyệt hủy vé. Hoàn tiền: {cancelRequest.RefundAmount:N0} VND."
            : $"Đã từ chối yêu cầu hủy. Lý do: {request.StaffNote}";

        return new CancelRequestResponseDto
        {
            CancelRequestId = cancelRequest.Id,
            TicketId = ticket.Id,
            PnrCode = booking.PnrCode,
            CancelStatus = cancelRequest.CancelStatus,
            TicketStatus = ticket.TicketStatus,
            RefundAmount = cancelRequest.RefundAmount,
            Message = message
        };
    }

    public async Task<List<CancelRequestResponseDto>> GetPendingCancelRequestsAsync(string currentRole)
    {
        if (!string.Equals(currentRole, AppConstants.StaffRoleName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chỉ Staff mới được xem danh sách yêu cầu hủy.");
        }

        var pendingRequests = await _dbContext.CancelRequests
            .Include(x => x.Ticket)
                .ThenInclude(x => x!.Booking)
            .Include(x => x.Ticket)
                .ThenInclude(x => x!.Flight)
            .Where(x => x.CancelStatus == AppConstants.CancelPendingStatus)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return pendingRequests.Select(cr => new CancelRequestResponseDto
        {
            CancelRequestId = cr.Id,
            TicketId = cr.TicketId,
            PnrCode = cr.Ticket?.Booking?.PnrCode ?? string.Empty,
            CancelStatus = cr.CancelStatus,
            TicketStatus = cr.Ticket?.TicketStatus ?? string.Empty,
            RefundAmount = cr.RefundAmount,
            Reason = cr.Reason,
            FlightNumber = cr.Ticket?.Flight?.FlightNumber,
            DepartureAirport = cr.Ticket?.Flight?.DepartureAirport,
            ArrivalAirport = cr.Ticket?.Flight?.ArrivalAirport,
            DepartureTime = cr.Ticket?.Flight?.DepartureTime,
            SeatClass = cr.Ticket?.SeatClass,
            RequestedAt = cr.CreatedAt,
            Message = $"Lý do hủy: {cr.Reason}"
        }).ToList();
    }
}
