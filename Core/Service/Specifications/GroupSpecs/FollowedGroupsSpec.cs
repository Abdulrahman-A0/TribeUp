using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class FollowedGroupsSpec : BaseSpecifications<Group, int>
    {
        public FollowedGroupsSpec(string userId, int page, int pageSize)
            : base(g => g.GroupFollowers.Any(f => f.UserId == userId))
        {
            AddIncludes(g => g.GroupScore);

            ApplyPagination(page, pageSize);

            AddOrderByDescending(g => g.CreatedAt);
        }
    }
}