using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberExistsException : ConflictException
    {
        public GroupMemberExistsException(string userId)
        : base($"User with id: '{userId}' already exists in this group") { }
    }
}
