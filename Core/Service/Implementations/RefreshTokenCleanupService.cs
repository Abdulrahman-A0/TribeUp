using Domain.Contracts;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;
using Service.Specifications.RefreshTokenSpecifications;
using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class RefreshTokenCleanupService(IUnitOfWork unitOfWork,
        ILogger<RefreshTokenCleanupService> logger) : IRefreshTokenCleanupService
    {
        public async Task CleanupExpiredTokensAsync()
        {
            try
            {

                var specification = new ExpiredRefreshTokensSpec();

                var repo = unitOfWork.GetRepository<RefreshToken, Guid>();

                var tokens = await repo.GetAllAsync(specification);

                if (!tokens.Any())
                    return;

                foreach (var token in tokens)
                    repo.Delete(token);

                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to cleanup expired refresh tokens");
                throw;
            }
        }
    }
}
