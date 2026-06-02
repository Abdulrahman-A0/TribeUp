using Domain.Entities.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.EventSpecifications
{
    public class GroupEventsSpecification: BaseSpecifications<Event, int>
    {
        public GroupEventsSpecification(int groupId)
            : base(e => e.GroupId == groupId)
        {
            AddOrderBy(e => e.EventDate);
            AddIncludes(e => e.Participants);
        }
    }
}
