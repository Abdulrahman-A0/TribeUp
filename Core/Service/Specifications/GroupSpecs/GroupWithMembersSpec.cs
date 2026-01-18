using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class GroupWithMembersSpec : BaseSpecifications<Group, int>
    {
        public GroupWithMembersSpec(int groupId) : base(g => g.Id == groupId)
        {
            AddIncludes(g => g.GroupMembers);
            AddThenIncludes(q => q.Include(g => g.GroupMembers)
                                            .ThenInclude(m => m.User));
        }
    }
}
