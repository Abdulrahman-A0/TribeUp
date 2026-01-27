using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.UserExceptions
{
    public sealed class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string userId)
            : base($"User '{userId}' was not found") { }
    }

}
