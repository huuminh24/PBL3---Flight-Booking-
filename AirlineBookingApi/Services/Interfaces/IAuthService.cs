using AirlineBookingApi.Models.DTOs.Auth;

namespace AirlineBookingApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<MeResponseDto?> GetMeAsync(int accountId);
    Task<MeResponseDto?> UpdateMeAsync(int accountId, UpdateMeRequestDto request);
    Task<string> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task ResetPasswordAsync(ResetPasswordRequestDto request);
}
