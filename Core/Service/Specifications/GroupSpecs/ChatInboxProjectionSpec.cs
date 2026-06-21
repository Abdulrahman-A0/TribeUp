using Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Service.Specifications.GroupSpecs
{
    public class ChatInboxProjectionSpec : BaseSpecifications<Group, int>
    {
        public ChatInboxProjectionSpec(string userId)
        : base(g => g.GroupMembers.Any(m => m.UserId == userId))
        {
            AddIncludes(g => g.LastMessage);
            AddThenIncludes(q =>
                q.Include(g => g.LastMessage)
                 .ThenInclude(m => m.User));

            AddOrderByDescending(g => g.LastMessageId != null);
            AddOrderByDescending(g => g.LastMessageSentAt);
            AddOrderByDescending(g => g.CreatedAt);
        }
    }
}