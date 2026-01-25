using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class GroupsWithMembersSpec : BaseSpecifications<Group, int>
    {
        public GroupsWithMembersSpec() : base(null)
        {
            AddIncludes(g => g.GroupMembers);
            AddThenIncludes(q => q.Include(g => g.GroupMembers)
                                   .ThenInclude(m => m.User));
        }
    }
}
