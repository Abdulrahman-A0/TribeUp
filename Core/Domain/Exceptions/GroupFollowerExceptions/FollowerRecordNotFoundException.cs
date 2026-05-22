using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GroupFollowerExceptions
{
    public class FollowerRecordNotFoundException : NotFoundException
    {
        public FollowerRecordNotFoundException(int followerId)
            : base($"Follower with ID `{followerId}` was not found.")
        {
        }
    }
}