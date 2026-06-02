using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.EventExceptions
{
    public class EventAlreadyCompletedException : ValidationException
    {
        public EventAlreadyCompletedException(int eventId)
            : base(new Dictionary<string, string[]>
            {
                ["Event"] = new[]
                {
                    $"Event with id '{eventId}' is already completed."
                }
            })
        {
        }
    }
}