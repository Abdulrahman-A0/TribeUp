using Domain.Entities.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PollSpecs
{
    public class UserPollVotesSpecification : BaseSpecifications<PollVote, int>
    {
        public UserPollVotesSpecification(int pollId, string userId)
            : base(v => v.PollId == pollId && v.UserId == userId)
        {
        }
    }
}
