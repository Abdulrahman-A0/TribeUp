using Domain.Exceptions.Abstraction;


namespace Domain.Exceptions.GroupFollowerExceptions
{
    public class GroupFollowerNotFoundException : NotFoundException
    {
        public GroupFollowerNotFoundException(string groupFollowerId)
            : base($"Group member with id `{groupFollowerId}` was not found.")
        {
        }
    }
}
