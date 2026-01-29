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
using Shared.DTOs.NotificationModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupJoinRequestService : IGroupJoinRequestService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IGroupScoreService groupScoreService;
        private readonly INotificationService notificationService;
        private readonly IGenericRepository<GroupJoinRequest, int> requestRepo;
        private readonly IGenericRepository<Group, int> groupRepo;
        private readonly IGenericRepository<GroupMember, int> memberRepo;
        public GroupJoinRequestService(IUnitOfWork unitOfWork, IMapper mapper, IGroupScoreService groupScoreService, INotificationService notificationService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.groupScoreService = groupScoreService;
            this.notificationService = notificationService;
            requestRepo = unitOfWork.GetRepository<GroupJoinRequest, int>();
            groupRepo = unitOfWork.GetRepository<Group, int>();
            memberRepo = unitOfWork.GetRepository<GroupMember, int>();
        }


        public async Task<GroupJoinRequestResultDTO> CreateJoinRequestAsync(int groupId, string userId)
        {
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);


            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var existingMember = await memberRepo.GetByIdAsync(memberSpec);
            if (existingMember != null)
                throw new GroupJoinRequestNotAllowedException("You are already a member of this group.");


            var requestSpec = new GroupJoinRequestByGroupAndUserSpec(groupId, userId);
            var existingRequest = await requestRepo.GetByIdAsync(requestSpec);
            if (existingRequest != null && existingRequest.Status == JoinRequestStatus.Pending)
                throw new GroupJoinRequestNotAllowedException("You already have a pending join request.");

            var request = new GroupJoinRequest
            {
                GroupId = groupId,
                UserId = userId,
                Status = JoinRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await requestRepo.AddAsync(request);
            await unitOfWork.SaveChangesAsync();

            var admins = await memberRepo.GetAllAsync(new GroupAdminsSpec(groupId));
            foreach (var admin in admins)
            {
                await notificationService.CreateAsync(new CreateNotificationDTO
                {
                    RecipientUserId = admin.UserId,
                    ActorUserId = userId,
                    Type = NotificationType.GroupJoinRequest,
                    Title = "New Join Request",
                    Message = "A user requested to join your group.",
                    ReferenceId = request.Id
                });
            }

            var details = await requestRepo.GetByIdAsync(
                new GroupJoinRequestWithDetailsSpec(request.Id));

            return mapper.Map<GroupJoinRequestResultDTO>(details);
        }




        public async Task<List<GroupJoinRequestResultDTO>> GetPendingRequestsAsync(int groupId, string userId)
        {
            var memberSpec = new GroupMemberByGroupAndUserSpec(groupId, userId);
            var member = await memberRepo.GetByIdAsync(memberSpec);
            if (member is null || member.Role != RoleType.Admin)
                throw new GroupAdminOnlyException();

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

            var memberSpec = new GroupMemberByGroupAndUserSpec(request.GroupId, userId);
            var member = await memberRepo.GetByIdAsync(memberSpec);
            if (member is null || member.Role != RoleType.Admin)
                throw new GroupAdminOnlyException();

            return mapper.Map<GroupJoinRequestResultDTO>(request);
        }



        public async Task<bool> ApproveJoinRequestAsync(int requestId, string userId)
        {
            var spec = new GroupJoinRequestWithDetailsSpec(requestId);
            var request = await requestRepo.GetByIdAsync(spec)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.Status != JoinRequestStatus.Pending)
                throw new GroupJoinRequestInvalidStateException();

            var adminSpec = new GroupMemberByGroupAndUserSpec(request.GroupId, userId);
            var adminMember = await memberRepo.GetByIdAsync(adminSpec);
            if (adminMember is null || adminMember.Role != RoleType.Admin)
                throw new GroupAdminOnlyException();

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
            await groupScoreService.IncreaseOnActionAsync(request.GroupId, 10);

            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = request.UserId,
                ActorUserId = userId,
                Type = NotificationType.GroupJoinApproved,
                Title = "Join Request Approved",
                Message = "Your request to join the group was approved.",
                ReferenceId = request.GroupId
            });

            return await unitOfWork.SaveChangesAsync() > 0;
        }




        public async Task<bool> RejectJoinRequestAsync(int requestId, string userId)
        {
            var spec = new GroupJoinRequestWithDetailsSpec(requestId);
            var request = await requestRepo.GetByIdAsync(spec)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.Status != JoinRequestStatus.Pending)
                throw new GroupJoinRequestInvalidStateException();

            var adminSpec = new GroupMemberByGroupAndUserSpec(request.GroupId, userId);
            var adminMember = await memberRepo.GetByIdAsync(adminSpec);
            if (adminMember is null || adminMember.Role != RoleType.Admin)
                throw new GroupAdminOnlyException();

            request.Status = JoinRequestStatus.Rejected;
            requestRepo.Update(request);


            await notificationService.CreateAsync(new CreateNotificationDTO
            {
                RecipientUserId = request.UserId,
                ActorUserId = userId,
                Type = NotificationType.GroupJoinRejected,
                Title = "Join Request Rejected",
                Message = "Your request to join the group was rejected.",
                ReferenceId = request.GroupId
            });

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



        public async Task<bool> CancelMyJoinRequestAsync(int requestId, string userId)
        {
            var request = await requestRepo.GetByIdAsync(requestId)
                ?? throw new GroupJoinRequestNoFoundException(requestId);

            if (request.UserId != userId)
                throw new GroupJoinRequestOwnershipException();

            if (request.Status != JoinRequestStatus.Pending)
                throw new GroupJoinRequestInvalidStateException();

            requestRepo.Delete(request);
            return await unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
