using Domain.Entities.Users;

namespace Service.Specifications.RefreshTokenSpecifications
{
    public class ActiveRefreshTokensByUserAndDeviceSpec : BaseSpecifications<RefreshToken, Guid>
    {
        public ActiveRefreshTokensByUserAndDeviceSpec(string userId, string deviceId)
            : base(rt => rt.UserId == userId && rt.DeviceId == deviceId && !rt.IsRevoked)
        {

        }
    }
}
