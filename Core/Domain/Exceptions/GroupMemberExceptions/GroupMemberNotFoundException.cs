using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberNotFoundException : NotFoundException
    {
        public GroupMemberNotFoundException(string UserId)
            : base($"User with Id `{UserId}` was not found in the group")
        {
        }
    }
}
