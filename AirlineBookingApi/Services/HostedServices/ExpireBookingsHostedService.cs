using AirlineBookingApi.Services.Interfaces;

namespace AirlineBookingApi.Services.HostedServices;

/// <summary>
/// Background service quét định kỳ và huỷ các booking PendingPayment đã quá hạn.
/// Giải phóng ghế đã reserve để khách khác có thể đặt lại.
/// </summary>
public class ExpireBookingsHostedService : BackgroundService
{
    private static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(5);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpireBookingsHostedService> _logger;

    public ExpireBookingsHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpireBookingsHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Đợi 30s đầu để app khởi động ổn định trước khi quét lần đầu.
        try { await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); }
        catch (OperationCanceledException) { return; }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var releasedCount = await bookingService.ReleaseExpiredBookingsAsync();

                if (releasedCount > 0)
                {
                    _logger.LogInformation("[ExpireBookings] Đã huỷ {Count} booking quá hạn và giải phóng ghế.", releasedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExpireBookings] Lỗi khi quét booking quá hạn.");
            }

            try { await Task.Delay(SweepInterval, stoppingToken); }
            catch (OperationCanceledException) { return; }
        }
    }
}
