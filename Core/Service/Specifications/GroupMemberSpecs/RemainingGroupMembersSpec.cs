using Domain.Entities.Groups;

namespace Service.Specifications.GroupMemberSpecs
{
    public class RemainingGroupMembersSpec : BaseSpecifications<GroupMembers, int>
    {
        public RemainingGroupMembersSpec(int groupId, string leavingUserId)
            : base(m => m.GroupId == groupId && m.UserId != leavingUserId)
        {
            AddOrderByDescending(m => m.Role);
            AddOrderBy(m => m.Id);
        }
    }
}