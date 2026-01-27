using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class GroupJoinRequestInvalidStateException
    : ConflictException
    {
        public GroupJoinRequestInvalidStateException()
            : base("Group join request is not in a pending state") { }
    }

}
