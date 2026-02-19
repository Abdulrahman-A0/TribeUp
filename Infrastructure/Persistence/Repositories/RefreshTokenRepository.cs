using Domain.Entities.Users;
using Domain.Contracts;
using Persistence.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class RefreshTokenRepository(AppDbContext context)
        : GenericRepository<RefreshToken, Guid>(context),
          IRefreshTokenRepository
    {
        public async Task<int> DeleteExpiredTokensAsync()
        {
            return await context.RefreshTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                .ExecuteDeleteAsync();
        }
    }
}
