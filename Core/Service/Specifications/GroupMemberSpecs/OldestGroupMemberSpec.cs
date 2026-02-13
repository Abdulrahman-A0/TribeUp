using Domain.Entities.Groups;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class OldestGroupMemberSpec : BaseSpecifications<GroupMembers, int>
    {
        public OldestGroupMemberSpec(int groupId)
            : base(m =>
                m.GroupId == groupId &&
                m.Role == RoleType.Admin)
        {
            AddOrderBy(m => m.JoinedAt);
        }
    }
}
