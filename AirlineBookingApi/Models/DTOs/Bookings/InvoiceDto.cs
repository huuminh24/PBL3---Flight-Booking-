namespace AirlineBookingApi.Models.DTOs.Bookings;

public class InvoiceDto
{
    public int BookingId { get; set; }
    public string PnrCode { get; set; } = string.Empty;
    public string BookingStatus { get; set; } = string.Empty;
    public string BookingChannel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string ContactFullName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;

    public List<InvoiceFlightDto> Flights { get; set; } = new();
    public List<InvoicePassengerDto> Passengers { get; set; } = new();
    public List<InvoiceTicketDto> Tickets { get; set; } = new();
    public InvoicePaymentDto? Payment { get; set; }

    public decimal TotalAmount { get; set; }
}

public class InvoiceFlightDto
{
    public int LegOrder { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string SeatClass { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class InvoicePassengerDto
{
    public int PassengerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string PassengerType { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
}

public class InvoiceTicketDto
{
    public int TicketId { get; set; }
    public string PassengerFullName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string SeatClass { get; set; } = string.Empty;
    public string? SeatNumber { get; set; }
    public bool IsCheckedIn { get; set; }
    public string TicketStatus { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class InvoicePaymentDto
{
    public int PaymentId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Amount { get; set; }
    public string? CouponCode { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime PaidAt { get; set; }
}
