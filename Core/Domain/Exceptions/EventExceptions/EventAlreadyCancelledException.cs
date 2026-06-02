using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.EventExceptions
{
    public class EventAlreadyCancelledException : ValidationException
    {
        public EventAlreadyCancelledException(int eventId)
            : base(new Dictionary<string, string[]>
            {
                ["Event"] = new[]
                {
                    $"Event with id '{eventId}' is already cancelled."
                }
            })
        {
        }
    }
}