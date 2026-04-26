using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupInvitaionSpecs
{
    public class ActiveGroupInvitationSpecification : BaseSpecifications<GroupInvitation, int>
    {
        public ActiveGroupInvitationSpecification(int groupId, string userId)
            : base(i => i.GroupId == groupId &&
                        i.UserId == userId &&
                        !i.IsRevoked &&
                        (i.ExpiresAt == null || i.ExpiresAt > DateTime.UtcNow))
        {
        }
    }
}
