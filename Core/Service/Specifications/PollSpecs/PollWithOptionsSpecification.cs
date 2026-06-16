using Domain.Entities.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PollSpecs
{
    public class PollWithOptionsSpecification : BaseSpecifications<Poll, int>
    {
        public PollWithOptionsSpecification(int pollId) : base(p => p.Id == pollId)
        {
            AddIncludes(p => p.PollOptions);
        }
    }
}
