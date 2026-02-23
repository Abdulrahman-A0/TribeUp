using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupMemberSpecs
{
    public class GroupMembersSpec : BaseSpecifications<GroupMembers, int>
    {
        public GroupMembersSpec(int groupId, int page, int pageSize, string? searchTerm = null)
            : base(m => m.GroupId == groupId &&
                   (string.IsNullOrEmpty(searchTerm) || m.User.UserName.ToLower().Contains(searchTerm.ToLower())))
        {
            AddIncludes(m => m.User);
            ApplyPagination(page, pageSize);
            AddOrderBy(m => m.JoinedAt);
        }
    }
}
