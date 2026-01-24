using Domain.Entities.Groups;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class ExploreGroupsSpec : BaseSpecifications<Group, int>
    {
        public ExploreGroupsSpec(int page, int pageSize)
            : base(g => g.Accessibility == AccessibilityType.Public)
        {
            AddIncludes(g => g.GroupMembers);
            AddIncludes(g => g.GroupScore);

            ApplyPagination(pageSize, page);
        }
    }
}
