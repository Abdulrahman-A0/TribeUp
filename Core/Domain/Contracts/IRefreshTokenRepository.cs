using Domain.Entities.Users;

namespace Domain.Contracts
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken, Guid>
    {
        Task<int> DeleteExpiredTokensAsync();
    }
}
