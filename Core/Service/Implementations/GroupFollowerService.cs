using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupFollowerExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Service.Specifications.GroupFollowerSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupFollowerModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupFollowerService : IGroupFollowerService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IUserGroupRelationService relationService;
        private readonly IGroupScoreService groupScoreService;
        private readonly INotificationService notificationService;
        private readonly IGenericRepository<GroupFollowers, int> followersRepo;


        public GroupFollowerService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IUserGroupRelationService relationService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.relationService = relationService;
            this.notificationService = notificationService;
            groupRepo = unitOfWork.GetRepository<Group, int>();
            followersRepo = unitOfWork.GetRepository<GroupFollowers, int>();
        }
        
        public async Task<FollowResultDTO> ToggleFollow(int groupId, string userId)
        {
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);
           
            if (relationService.IsMember(groupId))
            {
                throw new GroupMemberExistsException(userId);
            }
            else if(relationService.IsFollower(groupId))
            {
                var spec = new GroupFollwerByUserIdSpec(groupId, userId);
                var follower = await followersRepo.GetByIdAsync(spec)
                    ?? throw new GroupFollowerNotFoundException(userId);

                followersRepo.Delete(follower);
                await unitOfWork.SaveChangesAsync();
                return new FollowResultDTO
                {
                    Message = "Unfollowed"
                };
            }

            var newFollower = new GroupFollowers
            {
                UserId = userId,
                GroupId = groupId,
                FollowedAt = DateTime.UtcNow
            };

            await followersRepo.AddAsync(newFollower);
            await unitOfWork.SaveChangesAsync();

            return new FollowResultDTO
            {
                Message = "Followed"
            };
        }
    }
}
