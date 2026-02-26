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
        public ExploreGroupsSpec(int page, int pageSize, string userId, string? searchTerm)
            : base(g => g.Accessibility == AccessibilityType.Public &&
                       (string.IsNullOrWhiteSpace(searchTerm) ||
                        g.GroupName.ToLower().Contains(searchTerm.ToLower()) ||
                        g.Description.ToLower().Contains(searchTerm.ToLower())))
        {
            AddIncludes(g => g.GroupMembers);
            AddIncludes(g => g.GroupScore);

            AddOrderByDescending(g =>
                (g.GroupScore != null ? g.GroupScore.TotalPoints : 0) +
                (g.GroupMembers.Count * 5) -
                (g.GroupMembers.Any(m => m.UserId == userId) ? 1000 : 0)
            );

            ApplyPagination(page, pageSize);
        }
    }
}
