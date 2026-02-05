using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
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

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGroupJoinRequestService groupJoinRequestService;
        private readonly IGroupScoreService groupScoreService;
        private readonly INotificationService notificationService;
        private readonly IGroupAuthorizationService groupAuthorizationService;


        public GroupMemberService(IUnitOfWork unitOfWork, IMapper mapper, IGroupJoinRequestService groupJoinRequestService, IGroupScoreService groupScoreService, INotificationService notificationService, IGroupAuthorizationService groupAuthorizationService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.groupJoinRequestService = groupJoinRequestService;
            this.groupScoreService = groupScoreService;
            this.notificationService = notificationService;
            this.groupAuthorizationService = groupAuthorizationService;
        }



        public async Task<List<GroupMemberResultDTO>> GetGroupMembersAsync(int groupId, string userId)
        {
            
            await groupAuthorizationService.EnsureUserIsAdminAsync(groupId, userId);

            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();
            var spec = new GroupMembersSpec(groupId);
            var members = await memberRepo.GetAllAsync(spec);

            return mapper.Map<List<GroupMemberResultDTO>>(members);
        }



        public async Task<JoinGroupResponseDTO> JoinGroupAsync(int groupId, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);


            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var existingMember = await memberRepo.GetByIdAsync(memberSpec);
            if (existingMember != null)
                throw new GroupMemberExistsException(userId);

            var existingRequest =
                await groupJoinRequestService.GetUserRequestForGroupAsync(groupId, userId);

            if (existingRequest != null && existingRequest.Status == JoinRequestStatus.Pending)
                throw new GroupJoinRequestExistsException(existingRequest.Id);

            var request = await groupJoinRequestService.CreateJoinRequestAsync(groupId, userId);

            GroupMemberResultDTO? followerResult = null;

            if (group.Accessibility == AccessibilityType.Public)
            {
                var follower = new GroupMember
                {
                    GroupId = groupId,
                    UserId = userId,
                    Role = RoleType.Follower,
                    JoinedAt = DateTime.UtcNow
                };

                await memberRepo.AddAsync(follower);
                await unitOfWork.SaveChangesAsync();

                followerResult = mapper.Map<GroupMemberResultDTO>(follower);
            }

            return new JoinGroupResponseDTO
            {
                Message = group.Accessibility == AccessibilityType.Public
                    ? "You are now following this group. Awaiting admin approval."
                    : "Join request sent to group admins.",
                Request = request,
                Follower = followerResult
            };
        }



        
        public async Task<bool> LeaveGroupAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var leavingMember = await memberRepo.GetByIdAsync(memberSpec)
                ?? throw new UserNotMemberOfGroupException();

            memberRepo.Delete(leavingMember);

            if (leavingMember.Role != RoleType.Follower)
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

            if (leavingMember.Role == RoleType.Admin)
            {
                var adminsSpec = new GroupAdminsSpec(groupId);
                var admins = await memberRepo.GetAllAsync(adminsSpec);

                if (!admins.Any())
                {
                    var oldestMemberSpec = new OldestGroupMemberSpec(groupId);
                    var newAdmin = (await memberRepo.GetAllAsync(oldestMemberSpec))
                        .FirstOrDefault();

                    if (newAdmin != null)
                    {
                        newAdmin.Role = RoleType.Admin;
                        memberRepo.Update(newAdmin);
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

            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            await groupAuthorizationService.EnsureUserIsAdminAsync(groupId, actorUserId);

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
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            await groupAuthorizationService.EnsureUserIsAdminAsync(groupId, actorUserId);

            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.Role != RoleType.Admin)
                throw new InvalidGroupMemberRoleException(target.Role);

            if (target.UserId == actorUserId)
                throw new CannotDemoteSelfException();

            if (target.IsCreator)
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
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            await groupAuthorizationService.EnsureUserIsAdminAsync(groupId, actorUserId);

            var target = await memberRepo.GetByIdAsync(groupMemberId)
                ?? throw new GroupMemberNotFoundException(groupMemberId);

            if (target.GroupId != groupId)
                throw new GroupMemberNotInGroupException();

            if (target.UserId == actorUserId)
                throw new CannotKickSelfException();

            if (target.IsCreator)
                throw new CannotKickCreatorException();

            if (target.Role == RoleType.Admin)
            {
                var adminsSpec = new GroupAdminsSpec(groupId);
                var admins = await memberRepo.GetAllAsync(adminsSpec);

                if (admins.Count() <= 1)
                    throw new CannotRemoveLastAdminException();
            }

            memberRepo.Delete(target);

            if (target.Role != RoleType.Follower)
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
