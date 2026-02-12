using Domain.Entities.Groups;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class GroupAdminByUserSpec : BaseSpecifications<GroupMembers, int>
    {
        public GroupAdminByUserSpec(int groupId, string userId)
            : base(m =>
                m.GroupId == groupId &&
                m.UserId == userId &&
                m.Role == RoleType.Admin)
        {
        }
    }
}
