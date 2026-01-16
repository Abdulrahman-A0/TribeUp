using Domain.Entities.Users;

namespace Service.Specifications.RefreshTokenSpecifications
{
    public class ExpiredRefreshTokensSpec : BaseSpecifications<RefreshToken, Guid>
    {
        public ExpiredRefreshTokensSpec()
            : base(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
        {

        }
    }
}
