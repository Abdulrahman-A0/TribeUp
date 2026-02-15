using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.GroupSpecs
{
    public class ChatInboxProjectionSpec : BaseSpecifications<Group, int>
    {
        public ChatInboxProjectionSpec(string userId)
        : base(g =>
            g.LastMessageId != null &&
            g.GroupMembers.Any(m => m.UserId == userId))

        {
            AddIncludes(g => g.LastMessage);
            AddThenIncludes(q =>
                q.Include(g => g.LastMessage)
                 .ThenInclude(m => m.User));

            AddOrderByDescending(g => g.LastMessageSentAt);
        }
    }
}
