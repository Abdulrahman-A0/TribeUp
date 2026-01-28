using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupChatMessageSpecs
{
    public class GroupChatMessageWithUserSpec : BaseSpecifications<GroupChatMessage, long>
    {
        public GroupChatMessageWithUserSpec(long messageId) : base(m => m.Id == messageId)
        {
            AddIncludes(m => m.User);
        }
    }
}
