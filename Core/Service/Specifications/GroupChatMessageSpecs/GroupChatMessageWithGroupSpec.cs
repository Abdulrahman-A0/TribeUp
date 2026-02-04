using Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupChatMessageSpecs
{
    public class GroupChatMessageWithGroupSpec : BaseSpecifications<GroupChatMessage, long>
    {
        public GroupChatMessageWithGroupSpec(long messageId)
            : base(m => m.Id == messageId && !m.IsDeleted)
        {
            AddIncludes(m => m.User);
            AddIncludes(m => m.Group);
        }
    }
}
