using Domain.Contracts;
using Domain.Entities.Users;
using Service.Specifications.RefreshTokenSpecifications;
using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class RefreshTokenCleanupService(IUnitOfWork unitOfWork) : IRefreshTokenCleanupService
    {
        public async Task CleanupExpiredTokensAsync()
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
    }
}
