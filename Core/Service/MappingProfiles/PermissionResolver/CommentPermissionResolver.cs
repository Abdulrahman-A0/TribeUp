using AutoMapper;
using Domain.Entities.Posts;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction.Contracts;
using Shared.DTOs.CommentModule;
using Shared.DTOs.PostModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles.PermissionResolver
{
    public class CommentPermissionResolver
    : IValueResolver<Comment, CommentResultDTO, CommentPermissionDTO>
    {
        private readonly IUserGroupRelationService _relationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentPermissionResolver(
            IUserGroupRelationService relationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _relationService = relationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public CommentPermissionDTO Resolve(
            Comment source,
            CommentResultDTO destination,
            CommentPermissionDTO destMember,
            ResolutionContext context)
        {
            var currentUserId = _httpContextAccessor
                .HttpContext?.User?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return new();

            var groupId = source.Post.GroupId;

            var relation = _relationService.GetRelation(groupId);

            var isOwnerOfComment = source.UserId == currentUserId;

            return new CommentPermissionDTO
            {
                CanEdit = isOwnerOfComment,
                CanDelete =
                    isOwnerOfComment ||
                    relation == GroupRelationType.Admin ||
                    relation == GroupRelationType.Owner,
                CanModerate = 
                    relation == GroupRelationType.Admin ||
                    relation == GroupRelationType.Owner
            };
        }
    }
}
