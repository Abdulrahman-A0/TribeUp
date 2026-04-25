using Domain.Entities.Groups;

namespace Service.Specifications.GroupFollowerSpecs
{
    public class GroupFollowerByIdSpec : BaseSpecifications<GroupFollowers, int>
    {
        public GroupFollowerByIdSpec(int followerId)
            : base(f => f.Id == followerId)
        {
            AddIncludes(f => f.User);
        }
    }
}