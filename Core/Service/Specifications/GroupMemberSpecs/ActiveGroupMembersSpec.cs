using Domain.Entities.Groups;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class ActiveGroupMembersSpec : BaseSpecifications<GroupMembers, int>
    {
        public ActiveGroupMembersSpec(int groupId)
            : base(m =>
                m.GroupId == groupId &&
                (m.Role == RoleType.Admin || m.Role == RoleType.Member))
        {
        }
    }
}
