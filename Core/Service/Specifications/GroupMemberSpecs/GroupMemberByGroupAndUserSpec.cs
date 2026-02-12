using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class GroupMemberByGroupAndUserSpec : BaseSpecifications<GroupMembers, int>
    {
        public GroupMemberByGroupAndUserSpec(int groupId, string userId)
           : base(m => m.GroupId == groupId && m.UserId == userId)
        {
        }
    }
    
}
