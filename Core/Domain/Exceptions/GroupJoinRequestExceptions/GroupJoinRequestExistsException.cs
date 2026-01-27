using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class GroupJoinRequestExistsException
    : ConflictException
    {
        public GroupJoinRequestExistsException(int id)
            : base($"Join request with Id '{id}' already exists") { }
    }

}
