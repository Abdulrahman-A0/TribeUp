using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.Abstraction;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupInvitationExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.Extensions.Configuration;
using Service.Specifications.GroupFollowerSpecs;
using Service.Specifications.GroupInvitaionSpecs;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
using Shared.DTOs.NotificationModule;
using Shared.DTOs.Posts;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserGroupRelationService _relationService;
        private readonly IGroupScoreService _groupScoreService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public GroupInvitationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IGroupScoreService groupScoreService,
            INotificationService notificationService,
            IUserGroupRelationService relationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _groupScoreService = groupScoreService;
            _notificationService = notificationService;
            _relationService = relationService;
        }



        public async Task<InvitationResultDTO> CreateInvitationAsync(int groupId, string userId, CreateInvitationDTO dto)
        {
            if (!_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var activeSpec = new ActiveGroupInvitationSpecification(groupId, userId);
            var activeInvitation = await repo.GetByIdAsync(activeSpec);

            if (activeInvitation != null)
            {
                throw new ActiveInvitationAlreadyExistsException();
            }

            var expiredSpec = new ExpiredGroupInvitationSpecification(groupId, userId);
            var expiredInvitation = await repo.GetByIdAsync(expiredSpec);

            if (expiredInvitation != null)
            {
                expiredInvitation.IsRevoked = true;
            }

            var invitation = _mapper.Map<GroupInvitation>(dto);
            invitation.Token = Guid.NewGuid().ToString("N");
            invitation.GroupId = groupId;
            invitation.UserId = userId;
            invitation.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(invitation);
            await _unitOfWork.SaveChangesAsync();

            var frontUrl = _configuration["URLs:VercelUrl"];
            return _mapper.Map<InvitationResultDTO>(invitation, opt => opt.Items["FrontUrl"] = frontUrl);
        }





        public async Task<AcceptInvitationResponseDTO> AcceptInvitationAsync(string token, string userId)
        {
            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();
            var followerRepo = _unitOfWork.GetRepository<GroupFollowers, int>();

            var spec = new GetInvitationByTokenSpecification(token);
            var invitation = await repo.GetByIdAsync(spec);

            if (invitation == null)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Invalid invitation." } });

            if (_relationService.IsMember(invitation.GroupId))
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Already a member." } });

            if (invitation.IsRevoked)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Invitation revoked." } });

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt < DateTime.UtcNow)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Invitation expired." } });

            if (invitation.UsedCount >= invitation.MaxUses)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Invitation usage exceeded." } });

            bool wasFollower = _relationService.IsFollower(invitation.GroupId);

            if (wasFollower)
            {
                var followerSpec = new GroupFollowerByUserIdSpec(invitation.GroupId, userId);
                var follower = await followerRepo.GetByIdAsync(followerSpec);

                if (follower != null)
                    followerRepo.Delete(follower);
            }
            else
            {
                await _groupScoreService.IncreaseOnActionAsync(invitation.GroupId, 10);
            }

            var member = new GroupMembers
            {
                GroupId = invitation.GroupId,
                UserId = userId,
                Role = RoleType.Member
            };
            await _unitOfWork.GetRepository<GroupMembers, int>().AddAsync(member);

            invitation.UsedCount++;
            if (invitation.UsedCount >= invitation.MaxUses)
            {
                invitation.IsRevoked = true;
            }

            await SendJoinNotifications(invitation, userId);

            await _unitOfWork.SaveChangesAsync();

            return new AcceptInvitationResponseDTO { Success = true, Message = "Joined successfully." };
        }




        public async Task<InvitationDetailsDTO> GetInvitationDetailsAsync(string token)
        {
            var spec = new GetInvitationByTokenSpecification(token);
            var invitation = await _unitOfWork.GetRepository<GroupInvitation, int>()
                .GetByIdAsync(spec);

            if (invitation == null)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "Invalid invitation link." } });

            if (invitation.IsRevoked)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "This invitation has been revoked." } });

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt < DateTime.UtcNow)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "This invitation has expired." } });

            if (invitation.UsedCount >= invitation.MaxUses)
                throw new DomainValidationException(new Dictionary<string, string[]> { ["Invitation"] = new[] { "This invitation's usage limit has been exceeded." } });

            var dto = _mapper.Map<InvitationDetailsDTO>(invitation.Group);

            dto.MembersCount = await _unitOfWork
                                    .GetRepository<GroupMembers, int>()
                                    .CountAsync(m => m.GroupId == invitation.GroupId);

            return dto;
        }




        public async Task<InvitationResultDTO?> GetActiveInvitationAsync(int groupId, string userId)
        {
            if (!_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var spec = new ActiveGroupInvitationSpecification(groupId, userId);

            var invitation = await repo.GetByIdAsync(spec)
                             ?? throw new ActiveInvitationNotFoundException();

            var frontUrl = _configuration["URLs:VercelUrl"];

            return _mapper.Map<InvitationResultDTO>(invitation, opt =>
            {
                opt.Items["FrontUrl"] = frontUrl;
            });
        }




        public async Task<bool> RevokeInvitationAsync(int invitationId, string userId)
        {
            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var invitation = await repo.GetByIdAsync(invitationId)
                ?? throw new InvitationNotFoundException(invitationId);

            if (invitation.UserId != userId && !_relationService.IsOwner(invitation.GroupId))
            {
                throw new ForbiddenActionException();
            }

            if (invitation.IsRevoked)
            {
                return true;
            }

            invitation.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }





        private async Task SendJoinNotifications(GroupInvitation invitation, string joinedUserId)
        {
            var memberRepo = _unitOfWork.GetRepository<GroupMembers, int>();
            var adminsSpec = new GroupAdminsAndOwnerSpec(invitation.GroupId);
            var managers = await memberRepo.GetAllAsync(adminsSpec);

            var notificationDtos = managers.Select(manager => new CreateNotificationDTO
            {
                RecipientUserId = manager.UserId,
                ActorUserId = joinedUserId,
                Type = NotificationType.NewMemberJoined,
                Title = "New Member Joined",
                Message = $"A new member has joined '{invitation.Group?.GroupName ?? "the group"}' via {invitation.User?.UserName ?? "an administrator"}'s link.",
                ReferenceId = invitation.GroupId
            }).ToList();

            await _notificationService.CreateRangeAsync(notificationDtos);
        }

    }
}
