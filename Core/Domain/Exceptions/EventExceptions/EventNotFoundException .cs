using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.EventExceptions
{
    public class EventNotFoundException : NotFoundException
    {
        public EventNotFoundException(int eventId)
            : base($"Event with id '{eventId}' was not found.")
        {
        }
    }
}