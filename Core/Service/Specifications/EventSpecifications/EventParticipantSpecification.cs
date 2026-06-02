using Domain.Entities.Engagement;

namespace Service.Specifications.EventSpecifications
{
    public class EventParticipantSpecification: BaseSpecifications<EventParticipant, int>
    {
        public EventParticipantSpecification(
            int eventId,
            string userId)
            : base(x =>
                x.EventId == eventId &&
                x.UserId == userId)
        {
        }
    }
}
