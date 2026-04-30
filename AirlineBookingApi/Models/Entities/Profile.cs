namespace AirlineBookingApi.Models.Entities;

public class Profile : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }
}
