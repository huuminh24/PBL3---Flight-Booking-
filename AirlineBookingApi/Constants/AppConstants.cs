namespace AirlineBookingApi.Constants;

/// <summary>
/// Centralized constants for the entire application.
/// Eliminates magic strings scattered across services and controllers.
/// </summary>
public static class AppConstants
{
    // ── Role Names ──
    public const string CustomerRoleName = "Customer";
    public const string StaffRoleName = "Staff";

    // ── Booking Status ──
    public const string PendingPaymentStatus = "PendingPayment";
    public const string PaidStatus = "Paid";
    public const string FailedStatus = "Failed";
    public const string CancelledStatus = "Cancelled";
    public const string ConfirmedStatus = "Confirmed";

    // ── Ticket Status ──
    public const string CheckedInStatus = "CheckedIn";
    public const string CancelRequestedTicketStatus = "CancelRequested";

    // ── Cancel Request Status ──
    public const string CancelPendingStatus = "PendingApproval";
    public const string CancelApprovedStatus = "Approved";
    public const string CancelRejectedStatus = "Rejected";

    // ── Booking Channels ──
    public const string WebsiteChannel = "Website";
    public const string StaffChannel = "Staff";

    // ── Payment Methods ──
    public const string CashPaymentMethod = "Cash";
    public const string BankTransferPaymentMethod = "BankTransfer";
    public const string VietQRPaymentMethod = "VietQR";
    public const string CardPaymentMethod = "Card";
    /// <summary>Legacy alias kept so historical Payment rows remain readable. Do not use for new payments.</summary>
    public const string MockGatewayPaymentMethod = "MockGateway";

    // ── Check-in Window Status ──
    public const string CheckInWindowNotYetOpen = "NotYetOpen";
    public const string CheckInWindowOpen = "Open";
    public const string CheckInWindowClosed = "Closed";

    // ── Seat Classes ──
    public const string EconomySeatClass = "Economy";
    public const string BusinessSeatClass = "Business";

    // ── Passenger Types ──
    public const string AdultPassengerType = "Adult";
    public const string ChildPassengerType = "Child";
    public const string InfantPassengerType = "Infant";

    // ── Age Limits ──
    /// <summary>Người lớn từ 12 tuổi trở lên.</summary>
    public const int AdultMinAge = 12;
    /// <summary>Trẻ em từ 2 tuổi.</summary>
    public const int ChildMinAge = 2;
    /// <summary>Trẻ em dưới 12 tuổi.</summary>
    public const int ChildMaxAge = 11;
    /// <summary>Em bé dưới 2 tuổi.</summary>
    public const int InfantMaxAge = 1;

    // ── Validation ──
    /// <summary>Regex pattern for Vietnamese phone numbers (10-11 digits, starts with 0).</summary>
    public const string VietnamesePhoneRegex = @"^(0[3|5|7|8|9])[0-9]{8}$";

    // ── Business Rules ──
    /// <summary>Child ticket price multiplier (75% of adult fare).</summary>
    public const decimal ChildPriceMultiplier = 0.75m;
    /// <summary>Infant ticket price multiplier (10% of adult fare).</summary>
    public const decimal InfantPriceMultiplier = 0.10m;
    /// <summary>Business class refund rate (90%).</summary>
    public const decimal BusinessRefundRate = 0.9m;
    /// <summary>Economy class refund rate (50%).</summary>
    public const decimal EconomyRefundRate = 0.5m;
    /// <summary>Customer check-in window: max hours before departure.</summary>
    public const int CustomerCheckInMaxHours = 24;
    /// <summary>Customer check-in window: min hours before departure.</summary>
    public const int CustomerCheckInMinHours = 1;
    /// <summary>Booking payment expiry window in minutes from booking creation.</summary>
    public const int BookingPaymentExpiryMinutes = 15;
    /// <summary>Business class cancellation deadline (hours before departure).</summary>
    public const int BusinessCancelHours = 2;
    /// <summary>Economy class cancellation deadline (hours before departure).</summary>
    public const int EconomyCancelHours = 24;
    /// <summary>Hệ số oversell mặc định nếu không cấu hình trong appsettings (1.10 = cho phép bán 110% sức chứa).</summary>
    public const decimal DefaultOverbookingRatio = 1.10m;
    /// <summary>Tính số vé tối đa được phép bán dựa trên sức chứa thật và tỷ lệ oversell.</summary>
    public static int CalcMaxAllowedTickets(int totalRealSeats, decimal overbookingRatio)
    {
        if (totalRealSeats <= 0) return 0;
        if (overbookingRatio < 1m) overbookingRatio = 1m;
        return (int)Math.Floor(totalRealSeats * overbookingRatio);
    }

    // ── Flight Status (chuẩn hoá tên status mà Staff được phép set) ──
    public const string FlightScheduled = "Scheduled";
    public const string FlightBoarding = "Boarding";
    public const string FlightDeparted = "Departed";
    public const string FlightArrived = "Arrived";
    public const string FlightDelayed = "Delayed";
    public const string FlightCancelled = "Cancelled";

    // ── Ticket Status mở rộng cho oversell handling ──
    /// <summary>Khách không đến (no-show) — vé đã thanh toán nhưng không check-in trước cutoff.</summary>
    public const string NoShowStatus = "NoShow";

    // ── Hành động xử lý vé tồn (oversell) ──
    public const string OversellActionVoluntaryBump = "VoluntaryBump";
    public const string OversellActionNoShow = "NoShow";

    // ── Collections ──
    public static readonly string[] ValidSeatClasses = [EconomySeatClass, BusinessSeatClass];
    public static readonly string[] ValidPaymentMethods = [CashPaymentMethod, BankTransferPaymentMethod, VietQRPaymentMethod, CardPaymentMethod];
    public static readonly string[] ValidPassengerTypes = [AdultPassengerType, ChildPassengerType, InfantPassengerType];
    public static readonly string[] ValidGenders = ["Male", "Female"];
    public static readonly string[] ValidFlightStatuses = [FlightScheduled, FlightBoarding, FlightDeparted, FlightArrived, FlightDelayed, FlightCancelled];
}
