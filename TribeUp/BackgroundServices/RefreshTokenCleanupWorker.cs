
using ServiceAbstraction.Contracts;

namespace TribeUp.BackgroundServices
{
    public class RefreshTokenCleanupWorker(IServiceScopeFactory scopeFactory,
        ILogger<RefreshTokenCleanupWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var cleanupService = scope.ServiceProvider
                        .GetRequiredService<IRefreshTokenCleanupService>();

                    await cleanupService.CleanupExpiredTokensAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Refresh token cleanup failed");
                }

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
            }
        }
    }
}
