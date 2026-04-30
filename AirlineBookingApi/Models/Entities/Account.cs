namespace AirlineBookingApi.Models.Entities;

public class Account : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public Profile? Profile { get; set; }
}
