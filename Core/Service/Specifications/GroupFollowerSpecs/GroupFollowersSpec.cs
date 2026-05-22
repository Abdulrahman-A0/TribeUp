using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupFollowerSpecs
{
    public class GroupFollowersSpec : BaseSpecifications<GroupFollowers, int>
    {
        public GroupFollowersSpec(int groupId, int page, int pageSize, string? searchTerm)
            : base(f => f.GroupId == groupId && 
            (string.IsNullOrEmpty(searchTerm) || f.User.UserName.ToLower().Contains(searchTerm.ToLower())))
        {
            AddIncludes(f => f.User);
            ApplyPagination(page, pageSize);
            AddOrderByDescending(f => f.FollowedAt);
        }
    }
}
