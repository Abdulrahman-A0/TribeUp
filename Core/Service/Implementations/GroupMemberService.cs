using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupJoinRequestExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.NotificationModule;
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
        //private readonly IGroupAuthorizationService groupAuthorizationService;


        public GroupMemberService(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IGroupScoreService groupScoreService, 
            INotificationService notificationService, 
            IUserGroupRelationService relationService,
            IGroupAuthorizationService groupAuthorizationService)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.relationService = relationService;
            this.groupScoreService = groupScoreService;
            this.notificationService = notificationService;
        }



        public async Task<List<GroupMemberResultDTO>> GetGroupMembersAsync(int groupId, string userId)
        {

            if (!relationService.IsOwner(groupId) && !relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var spec = new GroupMembersSpec(groupId);
            var members = await memberRepo.GetAllAsync(spec);

            return mapper.Map<List<GroupMemberResultDTO>>(members);
        }




        public async Task<bool> LeaveGroupAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var leavingMember = await memberRepo.GetByIdAsync(memberSpec)
                ?? throw new UserNotMemberOfGroupException();

            memberRepo.Delete(leavingMember);

            if (relationService.GetRelation(groupId) >= GroupRelationType.Member)
                await groupScoreService.DecreaseOnActionAsync(groupId, 10);

            var activeMembersSpec = new ActiveGroupMembersSpec(groupId);
            var activeMembers = await memberRepo.GetAllAsync(activeMembersSpec);

            if (!activeMembers.Any())
            {
                var group = await groupRepo.GetByIdAsync(groupId);
                if (group != null)
                    groupRepo.Delete(group);

                return await unitOfWork.SaveChangesAsync() > 0;
            }

            if (relationService.GetRelation(groupId) == GroupRelationType.Owner)
            {
                var adminsSpec = new GroupAdminsSpec(groupId);
                var admins = await memberRepo.GetAllAsync(adminsSpec);

                if (!admins.Any())
                {
                    var oldestAdminSpec = new OldestGroupMemberSpec(groupId);
                    var newOwner = (await memberRepo.GetAllAsync(oldestAdminSpec))
                        .FirstOrDefault();

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
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();

            if (!relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

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
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);
            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();

            if (!relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.Role != RoleType.Admin)
                throw new InvalidGroupMemberRoleException(target.Role);

            if (target.UserId == actorUserId)
                throw new CannotDemoteSelfException();

            if (target.Role == RoleType.Owner)
                throw new CannotDemoteCreatorException();

            var adminsSpec = new GroupAdminsSpec(groupId);
            var admins = await memberRepo.GetAllAsync(adminsSpec);

            if (admins.Count() <= 1)
                throw new CannotRemoveLastAdminException();

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
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);
            var memberRepo = unitOfWork.GetRepository<GroupMembers, int>();

            if (!relationService.IsOwner(groupId) && !relationService.IsAdmin(groupId))
                throw new ForbiddenActionException();

            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.UserId == actorUserId)
                throw new CannotKickSelfException();

            if (target.Role == RoleType.Owner)
                throw new CannotKickCreatorException();

            if (target.Role == RoleType.Admin)
            {
                var adminsSpec = new GroupAdminsSpec(groupId);
                var admins = await memberRepo.GetAllAsync(adminsSpec);

                if (admins.Count() <= 1)
                    throw new CannotRemoveLastAdminException();
            }

            memberRepo.Delete(target);

            if (relationService.GetRelation(groupId) == GroupRelationType.Member)
                await groupScoreService.DecreaseOnActionAsync(groupId, 10);

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
