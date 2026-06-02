using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.EventExceptions
{
    public class EventResponseNotAllowedException : ValidationException
    {
        public EventResponseNotAllowedException(int eventId)
            : base(new Dictionary<string, string[]>
            {
                ["Event"] = new[]
                {
                    $"You cannot respond to event '{eventId}'."
                }
            })
        {
        }
    }
}