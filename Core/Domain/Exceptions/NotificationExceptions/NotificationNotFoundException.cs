using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.NotificationExceptions
{
    public sealed class NotificationNotFoundException
    : NotFoundException
    {
        public NotificationNotFoundException(int id)
            : base($"Notification '{id}' was not found") { }
    }

}
