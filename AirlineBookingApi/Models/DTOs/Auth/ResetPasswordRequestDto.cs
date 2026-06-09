using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Auth;

public class ResetPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string OtpCode { get; set; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.")]
    public string NewPassword { get; set; } = string.Empty;
}
