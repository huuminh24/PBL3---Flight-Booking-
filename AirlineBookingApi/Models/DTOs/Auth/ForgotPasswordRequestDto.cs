using System.ComponentModel.DataAnnotations;

namespace AirlineBookingApi.Models.DTOs.Auth;

public class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
