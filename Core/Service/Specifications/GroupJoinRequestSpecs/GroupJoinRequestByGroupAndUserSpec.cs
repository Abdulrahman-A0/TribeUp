using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupJoinRequestSpecs
{
    public class GroupJoinRequestByGroupAndUserSpec : BaseSpecifications<GroupJoinRequest, int>
    {
        // Checks if a user already has a request for a specific group
        public GroupJoinRequestByGroupAndUserSpec(int groupId, string userId)
            : base(r => r.GroupId == groupId && r.UserId == userId)
        {
            AddIncludes(r => r.User);
            AddIncludes(r => r.Group);
        }
    }
}
