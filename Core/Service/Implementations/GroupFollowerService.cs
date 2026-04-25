using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupFollowerExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Domain.Exceptions.ValidationExceptions;
using Service.Specifications.GroupFollowerSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupFollowerModule;
using Shared.DTOs.NotificationModule;
using Shared.DTOs.Posts;
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
            IGroupScoreService groupScoreService,
            IUserGroupRelationService relationService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.relationService = relationService;
            this.notificationService = notificationService;
            this.groupScoreService = groupScoreService;
            groupRepo = unitOfWork.GetRepository<Group, int>();
            followersRepo = unitOfWork.GetRepository<GroupFollowers, int>();
        }


        

        public async Task<PagedResult<GroupFollowerResultDTO>> GetGroupFollowersAsync(int groupId, int page, int pageSize, string? searchTerm = null)
        {
            if (!relationService.IsAdmin(groupId) && !relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = unitOfWork.GetRepository<GroupFollowers, int>();
            var spec = new GroupFollowersSpec(groupId, page, pageSize, searchTerm);
            var followers = await repo.GetAllAsync(spec);
            var totalCount = await repo.CountAsync(spec);

            var mappedFollowers = mapper.Map<List<GroupFollowerResultDTO>>(followers);

            return new PagedResult<GroupFollowerResultDTO>
            {
                Items = mappedFollowers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (page * pageSize) < totalCount
            };
        }



        public async Task<FollowActionResponseDTO> ToggleFollowAsync(int groupId, string userId)
        {
            var group = await groupRepo.GetByIdAsync(groupId)
                        ?? throw new GroupNotFoundException(groupId);

            if (relationService.IsMember(groupId))
                throw new MemberCannotFollowException();

            if (relationService.IsFollower(groupId))
            {
                var spec = new GroupFollowerByUserIdSpec(groupId, userId);
                var follower = (await followersRepo.GetAllAsync(spec)).FirstOrDefault()
                               ?? throw new GroupFollowerNotFoundException(userId);

                followersRepo.Delete(follower);

                await groupScoreService.DecreaseOnActionAsync(groupId, 10);

                await unitOfWork.SaveChangesAsync();

                return new FollowActionResponseDTO { Message = "Unfollowed", CurrentRelation = GroupRelationType.None };
            }

            var newFollower = new GroupFollowers { UserId = userId, GroupId = groupId };
            await followersRepo.AddAsync(newFollower);

            await groupScoreService.IncreaseOnActionAsync(groupId, 10);

            await unitOfWork.SaveChangesAsync();

            return new FollowActionResponseDTO { Message = "Followed", CurrentRelation = GroupRelationType.Follower };
        }



        public async Task RemoveFollowerAsync(int groupId, int followerId, string userId)
        {
            if (!relationService.IsOwner(groupId) && !relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var spec = new GroupFollowerByIdSpec(followerId);
            var follower = (await followersRepo.GetAllAsync(spec)).FirstOrDefault()
                        ?? throw new FollowerRecordNotFoundException(followerId);

            if (follower.GroupId != groupId)
                throw new ForbiddenActionException();

            followersRepo.Delete(follower);
            await groupScoreService.DecreaseOnActionAsync(groupId, 10);
            await unitOfWork.SaveChangesAsync();

            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = follower.UserId,
                ActorUserId = userId,
                Type = NotificationType.FollowerRemoved,
                Title = "Group Update",
                Message = "An administrator has removed you from the group's followers list.",
                ReferenceId = groupId
            });
        }
    }
}
