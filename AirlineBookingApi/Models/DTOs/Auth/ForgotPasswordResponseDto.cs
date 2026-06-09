namespace AirlineBookingApi.Models.DTOs.Auth;

public class ForgotPasswordResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
}
