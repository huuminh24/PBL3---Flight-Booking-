namespace AirlineBookingApi.Configurations;

/// <summary>
/// Cấu hình nghiệp vụ booking, đọc từ section "Booking" trong appsettings.
/// </summary>
public class BookingSettings
{
    public const string SectionName = "Booking";

    /// <summary>
    /// Tỷ lệ oversell (overbooking). 1.0 = chỉ bán đúng số ghế thật.
    /// 1.10 = bán tối đa 110% sức chứa, dôi 10% để bù no-show.
    /// </summary>
    public decimal OverbookingRatio { get; set; } = 1.10m;
}
