using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupChatMessageSpecs
{
    public class GroupChatMessageByIdSpec : BaseSpecifications<GroupChatMessage, long>
    {
        public GroupChatMessageByIdSpec(long messageId)
            : base(m => m.Id == messageId && !m.IsDeleted)
        {
        }
    }
}
