using Domain.Contracts;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using Service.Specifications.RefreshTokenSpecifications;
using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class RefreshTokenCleanupService(IRefreshTokenRepository tokenRepo,
        ILogger<RefreshTokenCleanupService> logger) : IRefreshTokenCleanupService
    {
        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                int deletedCount = await tokenRepo.DeleteExpiredTokensAsync();

                logger.LogInformation($"Cleaned up {deletedCount} expired refresh tokens.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to cleanup expired refresh tokens");
                throw;
            }
        }
    }
}
