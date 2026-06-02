using Domain.Entities.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.EventSpecifications
{
    public class EventByIdSpecification: BaseSpecifications<Event, int>
    {
        public EventByIdSpecification(int eventId)
            : base(e => e.Id == eventId)
        {
            AddIncludes(e => e.Participants);
        }
    }
}
