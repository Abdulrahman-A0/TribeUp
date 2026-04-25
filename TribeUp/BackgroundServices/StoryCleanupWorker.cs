
using ServiceAbstraction.Contracts;

namespace TribeUp.BackgroundServices
{
    public class StoryCleanupWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<StoryCleanupWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var cleanupService = scope.ServiceProvider
                        .GetRequiredService<IStoryCleanupService>();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred in StoryCleanupWorker.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
