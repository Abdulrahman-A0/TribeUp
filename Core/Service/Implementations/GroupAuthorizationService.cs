using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
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
    public class GroupAuthorizationService(IUnitOfWork unitOfWork, IMapper mapper) : IGroupAuthorizationService
    {
        // returns a member to be extendable
        // Later you might need:
        // member.Role → Admin-only actions
        // member.MutedUntil
        // member.JoinedAt
        // member.Nickname
        // member.IsBanned
        public async Task<GroupMemberResultDTO> EnsureUserCanChatAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var spec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var member = await memberRepo.GetByIdAsync(spec)
                ?? throw new UnauthorizedAccessException("You are not a member of this group.");

            if (member.Role is not RoleType.Member and not RoleType.Admin)
                throw new UnauthorizedAccessException("You are not allowed to chat in this group.");

            return mapper.Map<GroupMemberResultDTO>(member);
        }


        public async Task<GroupMemberResultDTO> EnsureUserIsAdminAsync(int groupId, string userId)
        {
            var memberRepo = unitOfWork.GetRepository<GroupMember, int>();

            var spec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var member = await memberRepo.GetByIdAsync(spec)
                ?? throw new UserNotMemberOfGroupException();

            if (member.Role != RoleType.Admin)
                throw new GroupAdminOnlyException();

            return mapper.Map<GroupMemberResultDTO>(member);
        }
    }
}
