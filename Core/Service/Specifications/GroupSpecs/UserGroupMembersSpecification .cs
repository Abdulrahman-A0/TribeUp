using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class UserGroupMembersSpecification : BaseSpecifications<GroupMembers, int>
    {
        public UserGroupMembersSpecification(string userId)
            : base(m => m.UserId == userId)
        {
        }
    }

}
