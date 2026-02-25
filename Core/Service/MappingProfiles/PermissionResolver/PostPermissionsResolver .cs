using AutoMapper;
using Domain.Entities.Posts;
using Service.Helper;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
using Shared.DTOs.Posts;

namespace Service.MappingProfiles.PermissionResolver
{
    public class PostPermissionsResolver
    : IValueResolver<Post, PostDTO, PostPermissionsDTO>
    {
        private readonly IUserGroupRelationService _relationService;

        public PostPermissionsResolver(
            IUserGroupRelationService relationService)
        {
            _relationService = relationService;
        }

        public PostPermissionsDTO Resolve(
            Post source,
            PostDTO destination,
            PostPermissionsDTO destMember,
            ResolutionContext context)
        {
            var relation = _relationService.GetRelation(source.GroupId);

            return PermissionBuilder.Build(relation);
        }
    }
}
