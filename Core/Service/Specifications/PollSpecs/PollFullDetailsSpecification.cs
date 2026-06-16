using Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PollSpecs
{
    public class PollFullDetailsSpecification : BaseSpecifications<Poll, int>
    {
        public PollFullDetailsSpecification(int pollId) : base(p => p.Id == pollId)
        {
            AddIncludes(p => p.CreatedByUser);

            AddThenIncludes(query => query
                .Include(p => p.PollOptions)
                .ThenInclude(o => o.PollVotes)
                .ThenInclude(v => v.User)
            );
        }
    }
}
