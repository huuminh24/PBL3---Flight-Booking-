using AirlineBookingApi.Models.Entities;

namespace AirlineBookingApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(Account account, string roleName);
    DateTime GetTokenExpiryTime();
}
