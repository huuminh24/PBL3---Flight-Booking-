using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.Entities;

public class CancelRequest : BaseEntity
{
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public int RequestedByAccountId { get; set; }
    public Account? RequestedByAccount { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string CancelStatus { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string? StaffNote { get; set; }

    /// <summary>Concurrency token — chống race khi 2 staff cùng duyệt 1 request.</summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
