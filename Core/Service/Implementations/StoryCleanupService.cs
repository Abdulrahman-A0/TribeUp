using Domain.Contracts;
using Domain.Entities.Stories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class StoryCleanupService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        ILogger<StoryCleanupService> logger) : IStoryCleanupService
    {
        public async Task CleanupExpiredStoriesAsync()
        {
            try
            {
                var storyRepo = unitOfWork.GetRepository<Story, int>();

                var expiredStoriesQuery = storyRepo.AsQueryable()
                    .Where(s => s.ExpiresAt <= DateTime.UtcNow);

                var mediaURLs = await expiredStoriesQuery
                    .Where(s => !string.IsNullOrEmpty(s.MediaURL))
                    .Select(s => s.MediaURL)
                    .ToListAsync();

                if (!mediaURLs.Any() && !await expiredStoriesQuery.AnyAsync())
                    return;

                foreach (var url in mediaURLs)
                {
                    try
                    {
                        await fileStorageService.DeleteAsync(url);
                    }
                    catch (Exception fileEx)
                    {
                        logger.LogWarning(fileEx, $"Failed to physically delete story media: {url}");
                    }
                }

                int deletedCount = await expiredStoriesQuery.ExecuteDeleteAsync();
                logger.LogInformation($"Cleaned up {deletedCount} expired stories and {mediaURLs.Count} media files.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while cleaning up expired stories.");
                throw;
            }
        }
    }
}
