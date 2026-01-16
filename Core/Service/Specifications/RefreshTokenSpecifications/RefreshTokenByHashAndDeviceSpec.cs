using Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.RefreshTokenSpecifications
{
    public class RefreshTokenByHashAndDeviceSpec : BaseSpecifications<RefreshToken, Guid>
    {
        public RefreshTokenByHashAndDeviceSpec(string tokenHash, string deviceId)
            : base(rt => rt.TokenHash == tokenHash && rt.DeviceId == deviceId)
        {
            AddIncludes(rt => rt.User);
        }
    }
}
