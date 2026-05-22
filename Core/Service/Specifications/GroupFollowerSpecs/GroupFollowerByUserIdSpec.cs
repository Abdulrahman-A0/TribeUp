using Domain.Entities.Groups;


namespace Service.Specifications.GroupFollowerSpecs
{
    public class GroupFollowerByUserIdSpec : BaseSpecifications<GroupFollowers, int>
    {
        public GroupFollowerByUserIdSpec(int groupId, string userId) 
            :base(f => f.GroupId == groupId && f.UserId == userId)
        {
        }
    }
}
