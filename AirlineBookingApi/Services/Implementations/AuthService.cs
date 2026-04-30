using AirlineBookingApi.Constants;
using AirlineBookingApi.Data;
using AirlineBookingApi.Helpers;
using AirlineBookingApi.Models.DTOs.Auth;
using AirlineBookingApi.Models.Entities;
using AirlineBookingApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AirlineBookingApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var existingAccount = await _dbContext.Accounts
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

        if (existingAccount is not null)
        {
            throw new InvalidOperationException("Email đã được sử dụng.");
        }

        var customerRole = await _dbContext.Roles
            .FirstOrDefaultAsync(x => x.Name == AppConstants.CustomerRoleName);

        if (customerRole is null)
        {
            throw new InvalidOperationException("Không tìm thấy role Customer trong hệ thống.");
        }

        var account = new Account
        {
            Email = normalizedEmail,
            PasswordHash = PasswordHasherHelper.HashPassword(request.Password),
            RoleId = customerRole.Id,
            IsActive = true,
            Profile = new Profile
            {
                FullName = request.FullName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address?.Trim()
            }
        };

        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.GenerateToken(account, customerRole.Name);

        return new AuthResponseDto
        {
            AccountId = account.Id,
            Email = account.Email,
            FullName = account.Profile?.FullName ?? string.Empty,
            Role = customerRole.Name,
            Token = token,
            ExpiresAt = _tokenService.GetTokenExpiryTime()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var account = await _dbContext.Accounts
            .Include(x => x.Role)
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

        if (account is null)
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

if (!account.IsActive)
  {
    throw new UnauthorizedAccessException("Tài khoản đã bị khóa.");
  }

  bool isValidPassword;
  try
  {
    isValidPassword = PasswordHasherHelper.VerifyPassword(request.Password, account.PasswordHash);
  }
  catch (Exception)
  {
    throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
  }

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

        var roleName = account.Role?.Name ?? string.Empty;
        var token = _tokenService.GenerateToken(account, roleName);

        return new AuthResponseDto
        {
            AccountId = account.Id,
            Email = account.Email,
            FullName = account.Profile?.FullName ?? string.Empty,
            Role = roleName,
            Token = token,
            ExpiresAt = _tokenService.GetTokenExpiryTime()
        };
    }

    public async Task<MeResponseDto?> GetMeAsync(int accountId)
    {
        var account = await _dbContext.Accounts
            .Include(a => a.Profile)
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.IsActive);

        if (account is null)
        {
            return null;
        }

        return new MeResponseDto
        {
            AccountId = account.Id,
            Email = account.Email,
            Role = account.Role?.Name ?? string.Empty,
            FullName = account.Profile?.FullName ?? string.Empty,
            PhoneNumber = account.Profile?.PhoneNumber ?? string.Empty,
            Address = account.Profile?.Address,
            DateOfBirth = account.Profile?.DateOfBirth
        };
    }

    public async Task<MeResponseDto?> UpdateMeAsync(int accountId, UpdateMeRequestDto request)
    {
        var account = await _dbContext.Accounts
            .Include(a => a.Profile)
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.IsActive);

        if (account is null)
        {
            return null;
        }

        account.Profile ??= new Profile { AccountId = account.Id };
        account.Profile.FullName = request.FullName.Trim();
        account.Profile.PhoneNumber = request.PhoneNumber.Trim();
        account.Profile.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        account.Profile.DateOfBirth = request.DateOfBirth;
        account.Profile.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new MeResponseDto
        {
            AccountId = account.Id,
            Email = account.Email,
            Role = account.Role?.Name ?? string.Empty,
            FullName = account.Profile.FullName,
            PhoneNumber = account.Profile.PhoneNumber,
            Address = account.Profile.Address,
            DateOfBirth = account.Profile.DateOfBirth
        };
    }
}
