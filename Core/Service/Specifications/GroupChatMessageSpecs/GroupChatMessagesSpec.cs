using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupChatMessageSpecs
{
    public class GroupChatMessagesSpec : BaseSpecifications<GroupChatMessage, long>
    {
        public GroupChatMessagesSpec(int groupId, int page, int pageSize)
            : base(m => m.GroupId == groupId && !m.IsDeleted)
        {
            AddIncludes(m => m.User);
            AddOrderByDescending(m => m.SentAt);
            ApplyPagination(pageSize, page);
        }
    }
}
