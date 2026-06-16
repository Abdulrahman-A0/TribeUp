using Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PollSpecs
{
    public class GroupPollsSpecification : BaseSpecifications<Poll, int>
    {
        public GroupPollsSpecification(int groupId, int pageIndex, int pageSize)
            : base(p => p.GroupId == groupId)
        {
            AddIncludes(p => p.CreatedByUser);

            AddThenIncludes(query => query
                .Include(p => p.PollOptions)
                .ThenInclude(o => o.PollVotes));

            AddOrderByDescending(p => p.CreatedAt);

            ApplyPagination(pageIndex, pageSize);
        }
    }
}
