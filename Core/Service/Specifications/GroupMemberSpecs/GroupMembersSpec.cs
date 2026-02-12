using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class GroupMembersSpec : BaseSpecifications<GroupMembers, int>
    {
        public GroupMembersSpec(int groupId)
            : base(m => m.GroupId == groupId)
        {
            AddIncludes(m => m.User);
            AddOrderBy(m => m.JoinedAt);
        }
    }
}
