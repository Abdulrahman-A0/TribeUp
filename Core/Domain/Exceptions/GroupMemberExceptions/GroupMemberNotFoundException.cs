using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupMemberExceptions
{
    public class GroupMemberNotFoundException : NotFoundException
    {
        public GroupMemberNotFoundException(int groupMemberId)
        : base($"Group member with id `{groupMemberId}` was not found.")
        {
        }
    }
}
