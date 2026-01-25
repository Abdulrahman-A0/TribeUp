using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupJoinRequestExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMemberModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupMemberService : IGroupMemberService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGroupJoinRequestService groupJoinRequestService;
        private readonly IGroupScoreService groupScoreService;

        public GroupMemberService(IUnitOfWork unitOfWork,IMapper mapper,IGroupJoinRequestService groupJoinRequestService, IGroupScoreService groupScoreService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.groupJoinRequestService = groupJoinRequestService;
            this.groupScoreService = groupScoreService;
        }

        // JoinGroup
        public async Task<JoinGroupResponseDTO> JoinOrRequestGroupAsync(int groupId, string userId)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var existingMember = await memberRepo.GetByIdAsync(memberSpec);
            if (existingMember != null)
                throw new GroupMemberExistsException(existingMember.UserId);

            if (group.Accessibility == AccessibilityType.Public)
            {
                var member = new GroupMember
                {
                    GroupId = groupId,
                    UserId = userId,
                    Role = RoleType.Member,
                    JoinedAt = System.DateTime.UtcNow
                };

                await memberRepo.AddAsync(member);
                await groupScoreService.IncreaseOnActionAsync(groupId, 10);
                await unitOfWork.SaveChangesAsync();

                var memberResult = mapper.Map<GroupMemberResultDTO>(member);

                return new JoinGroupResponseDTO
                {
                    IsImmediate = true,
                    Message = "You have successfully joined the group!",
                    MemberResult = memberResult,
                    RequestResult = null
                };
            }
            else
            {
                var hasPendingRequest =
                    await groupJoinRequestService.GetUserRequestForGroupAsync(groupId, userId);
                if (hasPendingRequest is not null)
                    throw new GroupJoinRequestExistsException(hasPendingRequest.Id);

                var requestResult = await groupJoinRequestService.CreateJoinRequestAsync(groupId, userId);

                return new JoinGroupResponseDTO
                {
                    IsImmediate = false,
                    Message = "This is a private group. Your join request has been sent to the admins for approval.",
                    MemberResult = null,
                    RequestResult = requestResult
                };
            }
        }



        // LeaveGroup
        public async Task<bool> LeaveGroupAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();
            var groupRepo = unitOfWork.GetRepository<Group, int>();


            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var leavingMember = await memberRepo.GetByIdAsync(memberSpec)
                ?? throw new GroupMemberNotFoundException(userId);

            var membersSpec = new GroupMembersInGroupSpec(groupId);
            var members = (await memberRepo.GetAllAsync(membersSpec)).ToList();


            bool isAdmin = leavingMember.Role == RoleType.Admin;
            int adminCount = members.Count(m => m.Role == RoleType.Admin);

            memberRepo.Delete(leavingMember);
            members.Remove(leavingMember);

            await groupScoreService.DecreaseOnActionAsync(groupId, 10);

            if (!members.Any())
            {
                var group = await groupRepo.GetByIdAsync(groupId);
                if (group != null)
                    groupRepo.Delete(group);

                return await unitOfWork.SaveChangesAsync() > 0;
            }

            if (isAdmin && adminCount == 1)
            {
                var newAdmin = members
                    .OrderBy(m => m.JoinedAt)
                    .First();

                newAdmin.Role = RoleType.Admin;
                memberRepo.Update(newAdmin);
            }

            return await unitOfWork.SaveChangesAsync() > 0;
        }

    }
}
