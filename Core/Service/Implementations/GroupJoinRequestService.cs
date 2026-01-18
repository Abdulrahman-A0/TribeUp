using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using Domain.Exceptions.GroupJoinRequestExceptions;
using Domain.Exceptions.GroupMemberExceptions;
using Service.Specifications.GroupJoinRequestSpecs;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupJoinRequestModule;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupJoinRequestService : IGroupJoinRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGenericRepository<GroupJoinRequest, int> requestRepo;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IGenericRepository<GroupMember, int> memberRepo;

        public GroupJoinRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            requestRepo = unitOfWork.GetRepository<GroupJoinRequest, int>();
            groupRepo = unitOfWork.GetRepository<Group, int>();
            memberRepo = unitOfWork.GetRepository<GroupMember, int>();
        }


        public async Task<GroupJoinRequestResultDTO> CreateJoinRequestAsync(int groupId, string userId)
        {
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            if (group.Accessibility != AccessibilityType.Private)
                throw new InvalidGroupJoinRequestException(groupId, group.Accessibility.ToString());

            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var existingMember = await memberRepo.GetByIdAsync(memberSpec);
            if (existingMember != null)
                throw new GroupMemberExistsException(userId);


            var requestSpec = new GroupJoinRequestByGroupAndUserSpec(groupId, userId);
            var existingRequest = await requestRepo.GetByIdAsync(requestSpec);
            if (existingRequest != null && existingRequest.Status != JoinRequestStatus.Rejected)
                throw new GroupJoinRequestExistsException(existingRequest.Id);

            var newRequest = new GroupJoinRequest
            {
                GroupId = groupId,
                UserId = userId,
                Status = JoinRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await requestRepo.AddAsync(newRequest);
            await unitOfWork.SaveChangesAsync();

            var detailsSpec = new GroupJoinRequestWithDetailsSpec(newRequest.Id);
            var result = await requestRepo.GetByIdAsync(detailsSpec);
            return mapper.Map<GroupJoinRequestResultDTO>(result);
        }


        public async Task<List<GroupJoinRequestResultDTO>> GetPendingRequestsAsync(int groupId, string userId)
        {
            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var member = await memberRepo.GetByIdAsync(memberSpec);
            if (member is null || member.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("Only group admins can view pending requests.");

            var spec = new PendingGroupJoinRequestsSpec(groupId);
            var requests = await requestRepo.GetAllAsync(spec);

            return mapper.Map<List<GroupJoinRequestResultDTO>>(requests.ToList());
        }

        public async Task<GroupJoinRequestResultDTO> GetRequestByIdAsync(int requestId, string userId)
        {
            var spec = new GroupJoinRequestWithDetailsSpec(requestId);
            var request = await requestRepo.GetByIdAsync(spec)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.UserId == userId)
                return mapper.Map<GroupJoinRequestResultDTO>(request);

            var memberSpec = new GroupMemberByGroupAndUserSpec(request.GroupId,userId);
            var member = await memberRepo.GetByIdAsync(memberSpec);
            if (member is null || member.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("You are not allowed to access this request");

            return mapper.Map<GroupJoinRequestResultDTO>(request);
        }


        public async Task<bool> ApproveJoinRequestAsync(int requestId, string userId)
        {
            var spec = new GroupJoinRequestWithDetailsSpec(requestId);
            var request = await requestRepo.GetByIdAsync(spec)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.Status != JoinRequestStatus.Pending)
                throw new InvalidOperationException("Request is not pending.");

            var adminSpec = new GroupMemberByGroupAndUserSpec(request.GroupId, userId);
            var adminMember = await memberRepo.GetByIdAsync(adminSpec);
            if (adminMember is null || adminMember.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("Only group admins can approve join requests.");

            request.Status = JoinRequestStatus.Approved;
            requestRepo.Update(request);

            var newMember = new GroupMember
            {
                GroupId = request.GroupId,
                UserId = request.UserId,
                Role = RoleType.Member,
                JoinedAt = DateTime.UtcNow
            };

            await memberRepo.AddAsync(newMember);
            return await unitOfWork.SaveChangesAsync() > 0;
        }


        public async Task<bool> RejectJoinRequestAsync(int requestId, string userId)
        {
            var spec = new GroupJoinRequestWithDetailsSpec(requestId);
            var request = await requestRepo.GetByIdAsync(spec)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.Status != JoinRequestStatus.Pending)
                throw new InvalidOperationException("Request is not pending.");

            var adminSpec = new GroupMemberByGroupAndUserSpec(request.GroupId, userId);
            var adminMember = await memberRepo.GetByIdAsync(adminSpec);
            if (adminMember is null || adminMember.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("Only group admins can reject join requests.");

            request.Status = JoinRequestStatus.Rejected;
            requestRepo.Update(request);

            return await unitOfWork.SaveChangesAsync() > 0;
        }


        public async Task<List<GroupJoinRequestResultDTO>> GetUserRequestsAsync(string userId)
        {
            var spec = new AllGroupJoinRequestsByUserSpec(userId);
            var requests = await requestRepo.GetAllAsync(spec);

            return mapper.Map<List<GroupJoinRequestResultDTO>>(requests.ToList());
        }


        public async Task<GroupJoinRequestResultDTO?> GetUserRequestForGroupAsync(int groupId, string userId)
        {
            var spec = new GroupJoinRequestByGroupAndUserSpec(groupId, userId);
            var request = await requestRepo.GetByIdAsync(spec);

            return request == null ? null : mapper.Map<GroupJoinRequestResultDTO>(request);
        }
    }
}
