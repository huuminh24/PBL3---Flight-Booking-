namespace AirlineBookingApi.Models.DTOs.Staff;

public class StaffCustomerLookupDto
{
    public int AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
