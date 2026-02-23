using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.NotificationModule;
using Shared.DTOs.Posts;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupMemberService : IGroupMemberService
    {

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGroupScoreService groupScoreService;
        private readonly INotificationService notificationService;
        private readonly IUserGroupRelationService relationService;


        public GroupMemberService(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IGroupScoreService groupScoreService, 
            INotificationService notificationService, 
            IUserGroupRelationService relationService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.relationService = relationService;
            this.groupScoreService = groupScoreService;
            this.notificationService = notificationService;
        }




        public async Task<PagedResult<GroupMemberResultDTO>> GetGroupMembersAsync(int groupId, string userId, int page, int pageSize, string? searchTerm = null)
        {
            if (!relationService.IsOwner(groupId) && !relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var spec = new GroupMembersSpec(groupId, page, pageSize, searchTerm);
            var members = await memberRepo.GetAllAsync(spec);

            var totalCount = await memberRepo.CountAsync(m => m.GroupId == groupId &&
            (string.IsNullOrEmpty(searchTerm) || m.User.UserName.ToLower().Contains(searchTerm.ToLower())));

            var mappedMembers = mapper.Map<List<GroupMemberResultDTO>>(members);

            return new PagedResult<GroupMemberResultDTO>
            {
                Items = mappedMembers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (page * pageSize) < totalCount
            };
        }





        public async Task<bool> LeaveGroupAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();

            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var leavingMember = await memberRepo.GetByIdAsync(memberSpec)
                ?? throw new UserNotMemberOfGroupException();

            if (leavingMember.Role >= RoleType.Member)
            {
                await groupScoreService.DecreaseOnActionAsync(groupId, 10);
            }

            memberRepo.Delete(leavingMember);

            var activeMembersSpec = new ActiveGroupMembersSpec(groupId);
            var activeMembers = await memberRepo.GetAllAsync(activeMembersSpec);

            var groupRepo = unitOfWork.GetRepository<Group, int>();
            if (!activeMembers.Any())
            {
                var group = await groupRepo.GetByIdAsync(groupId);
                if (group != null) groupRepo.Delete(group);
                return await unitOfWork.SaveChangesAsync() > 0;
            }

            if (leavingMember.Role == RoleType.Owner)
            {
                var adminsSpec = new GroupAdminsSpec(groupId);
                var admins = await memberRepo.GetAllAsync(adminsSpec);

                if (!admins.Any())
                {
                    var oldestMemberSpec = new OldestGroupMemberSpec(groupId);
                    var newOwner = (await memberRepo.GetAllAsync(oldestMemberSpec)).FirstOrDefault();

                    if (newOwner != null)
                    {
                        newOwner.Role = RoleType.Owner;
                        memberRepo.Update(newOwner);
                    }
                }
            }

            return await unitOfWork.SaveChangesAsync() > 0;
        }





        public async Task<bool> PromoteToAdminAsync(int groupId, string actorUserId, int groupMemberId)
        {
            if (!relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.Role != RoleType.Member)
                throw new InvalidGroupMemberRoleException(target.Role);

            target.Role = RoleType.Admin;
            memberRepo.Update(target);

            await unitOfWork.SaveChangesAsync();

            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = target.UserId,
                ActorUserId = actorUserId,
                Type = NotificationType.GroupMemberRoleChanged,
                Title = "Promotion",
                Message = $"You have been promoted to admin in {group.GroupName} group.",
                ReferenceId = groupId
            });

            return true;
        }






        public async Task<bool> DemoteAdminAsync(int groupId, string actorUserId, int groupMemberId)
        {
            if (!relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.UserId == actorUserId)
                throw new CannotDemoteSelfException();

            if (target.Role == RoleType.Owner)
                throw new CannotDemoteCreatorException();

            target.Role = RoleType.Member;
            memberRepo.Update(target);

            await unitOfWork.SaveChangesAsync();

            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = target.UserId,
                ActorUserId = actorUserId,
                Type = NotificationType.GroupMemberRoleChanged,
                Title = "Demotion",
                Message = $"You have been demoted to member in {group.GroupName} group.",
                ReferenceId = groupId
            });

            return true;
        }





        public async Task<bool> KickMemberAsync(int groupId, string actorUserId, int groupMemberId)
        {
            if (!relationService.IsOwner(groupId) && !relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.UserId == actorUserId)
                throw new CannotKickSelfException();

            if (target.Role == RoleType.Owner)
                throw new CannotKickCreatorException();

            if (target.Role == RoleType.Admin && !relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            if (target.Role == RoleType.Member || target.Role == RoleType.Admin)
            {
                await groupScoreService.DecreaseOnActionAsync(groupId, 10);
            }

            memberRepo.Delete(target);

            await unitOfWork.SaveChangesAsync();

            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = target.UserId,
                ActorUserId = actorUserId,
                Type = NotificationType.GroupMemberRemoved,
                Title = "Kick Out",
                Message = $"You have been removed from {group.GroupName} group.",
                ReferenceId = groupId
            });

            return true;
        }

    }
}
