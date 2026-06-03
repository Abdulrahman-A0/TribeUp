using Domain.Entities.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PollSpecs
{
    public class GroupPollsCountSpecification : BaseSpecifications<Poll, int>
    {
        public GroupPollsCountSpecification(int groupId)
            : base(p => p.GroupId == groupId)
        {
        }
    }
}
