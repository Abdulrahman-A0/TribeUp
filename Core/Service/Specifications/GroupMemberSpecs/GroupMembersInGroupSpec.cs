using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class GroupMembersInGroupSpec : BaseSpecifications<GroupMembers, int>
    {
        public GroupMembersInGroupSpec(int groupId)
            : base(m => m.GroupId == groupId)
        {
        }
    }
}
