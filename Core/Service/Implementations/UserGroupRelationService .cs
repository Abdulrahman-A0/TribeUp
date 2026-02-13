using Domain.Contracts;
using Domain.Entities.Groups;
using Service.Specifications.GroupSpecs;
using ServiceAbstraction.Contracts;
using Shared.Enums;

namespace Service.Implementations
{
    public class UserGroupRelationService : IUserGroupRelationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private Dictionary<int, GroupRelationType> _relations = new();

        private string _userId = string.Empty;

        public UserGroupRelationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task InitializeAsync(string userId)
        {
            _userId = userId;

            var memberRepo = _unitOfWork.GetRepository<GroupMembers, int>();
            var followerRepo = _unitOfWork.GetRepository<GroupFollowers, int>();

            var memberSpec = new UserGroupMembersSpecification(userId);
            var members = await memberRepo.GetAllAsync(memberSpec);

            foreach (var m in members)
            {
                _relations[m.GroupId] = m.Role switch
                {
                    RoleType.Owner => GroupRelationType.Owner,
                    RoleType.Admin => GroupRelationType.Admin,
                    RoleType.Member => GroupRelationType.Member,
                    _ => GroupRelationType.None
                };
            }

            var followerSpec = new UserGroupFollowersSpecification(userId);
            var followers = await followerRepo.GetAllAsync(followerSpec);

            foreach (var f in followers)
            {
                if (!_relations.ContainsKey(f.GroupId))
                    _relations[f.GroupId] = GroupRelationType.Follower;
            }
        }

        public GroupRelationType GetRelation(int groupId)
            => _relations.TryGetValue(groupId, out var relation)
                ? relation
                : GroupRelationType.None;
        public bool IsAdmin(int groupId)
            => GetRelation(groupId) == GroupRelationType.Admin;
        public bool IsOwner(int groupId)
            => GetRelation(groupId) == GroupRelationType.Owner;
        public bool IsMember(int groupId)
            => GetRelation(groupId) >= GroupRelationType.Member;
        public bool IsFollower(int groupId)
            => GetRelation(groupId) == GroupRelationType.Follower;
        public bool IsNone(int groupId)
            => GetRelation(groupId) == GroupRelationType.None;

    }

}
