using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.CancelRequests;

/// <summary>
/// DTO để Staff duyệt hoặc từ chối yêu cầu hủy vé.
/// </summary>
public class ProcessCancelRequestDto
{
    /// <summary>true = Duyệt (Approved), false = Từ chối (Rejected)</summary>
    [Required]
    public bool IsApproved { get; set; }

    /// <summary>Lý do từ chối (bắt buộc nếu từ chối)</summary>
    public string? StaffNote { get; set; }
}
