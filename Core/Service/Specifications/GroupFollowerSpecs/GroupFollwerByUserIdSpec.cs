using Domain.Entities.Groups;


namespace Service.Specifications.GroupFollowerSpecs
{
    public class GroupFollwerByUserIdSpec : BaseSpecifications<GroupFollowers, int>
    {
        public GroupFollwerByUserIdSpec(int groupId, string userId) 
            :base(f => f.GroupId == groupId && f.UserId == userId)
        {
        }
    }
}
