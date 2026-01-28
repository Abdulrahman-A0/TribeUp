using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.NotificationExceptions
{
    public sealed class NotificationAccessDeniedException
    : UnauthorizedDomainException
    {
        public NotificationAccessDeniedException()
            : base("You are not allowed to access this notification") { }
    }

}
