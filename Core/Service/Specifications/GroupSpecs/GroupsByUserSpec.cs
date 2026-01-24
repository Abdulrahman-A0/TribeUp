using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class GroupsByUserSpec : BaseSpecifications<Group, int>
    {
        public GroupsByUserSpec(string userId)
            : base(g => g.GroupMembers.Any(m => m.UserId == userId))
        {
            AddIncludes(g => g.GroupMembers);
            AddIncludes(g => g.GroupScore);
        }
    }
}
