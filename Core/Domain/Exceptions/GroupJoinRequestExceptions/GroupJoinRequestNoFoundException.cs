using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class GroupJoinRequestNoFoundException : NotFoundException
    {
        public GroupJoinRequestNoFoundException(int requestId)
            : base($"Group join request with id: {requestId} not found")
        {
        }
    }
}
