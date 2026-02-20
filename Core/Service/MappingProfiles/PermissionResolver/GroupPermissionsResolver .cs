using AutoMapper;
using Domain.Entities.Posts;
using Service.Helper;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
using Shared.DTOs.Posts;

namespace Service.MappingProfiles.PermissionResolver
{
    public class GroupPermissionsResolver
    : IValueResolver<Post, PostDTO, GroupPermissionsDTO>
    {
        private readonly IUserGroupRelationService _relationService;

        public GroupPermissionsResolver(
            IUserGroupRelationService relationService)
        {
            _relationService = relationService;
        }

        public GroupPermissionsDTO Resolve(
            Post source,
            PostDTO destination,
            GroupPermissionsDTO destMember,
            ResolutionContext context)
        {
            var relation = _relationService.GetRelation(source.GroupId);

            return PermissionBuilder.Build(relation);
        }
    }
}
