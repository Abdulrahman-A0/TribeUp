using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.Abstraction;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupInvitationExceptions;
using Domain.Exceptions.ValidationExceptions;
using Microsoft.Extensions.Configuration;
using Service.Specifications.GroupInvitaionSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
using Shared.DTOs.Posts;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserGroupRelationService _relationService;
        private readonly IConfiguration _configuration;

        public GroupInvitationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            IUserGroupRelationService relationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _relationService = relationService;
        }

        


        public async Task<InvitationResultDTO> CreateInvitationAsync(int groupId, string userId, CreateInvitationDTO dto)
        {
            if (!_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var spec = new ActiveGroupInvitationSpecification(groupId);
            var activeInvitation = (await repo.GetAllAsync(spec)).FirstOrDefault();

            if (activeInvitation != null)
                throw new ActiveInvitationAlreadyExistsException();

            var invitation = _mapper.Map<GroupInvitation>(dto);

            invitation.Token = Guid.NewGuid().ToString("N");
            invitation.GroupId = groupId;
            invitation.UserId = userId;

            await repo.AddAsync(invitation);
            await _unitOfWork.SaveChangesAsync();

            var frontUrl = _configuration["URLs:NetlifyUrl"];
            return _mapper.Map<InvitationResultDTO>(invitation, opt => opt.Items["FrontUrl"] = frontUrl);
        }





        public async Task<AcceptInvitationResponseDTO> AcceptInvitationAsync(
            string token,
            string userId)
        {
            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var spec = new GetInvitationByTokenSpecification(token);
            var invitation = (await repo.GetAllAsync(spec)).FirstOrDefault();

            if (invitation == null)
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Invitation"] = new[] { "Invalid invitation." }
                    });

            if (invitation.IsRevoked)
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Invitation"] = new[] { "Invitation revoked." }
                    });

            if (invitation.ExpiresAt.HasValue &&
                invitation.ExpiresAt < DateTime.UtcNow)
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Invitation"] = new[] { "Invitation expired." }
                    });

            if (invitation.MaxUses.HasValue &&
                invitation.UsedCount >= invitation.MaxUses)
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Invitation"] = new[] { "Invitation usage exceeded." }
                    });

            if (_relationService.IsMember(invitation.GroupId))
                throw new DomainValidationException(
                    new Dictionary<string, string[]>
                    {
                        ["Invitation"] = new[] { "Already a member." }
                    });

            var member = new GroupMembers
            {
                GroupId = invitation.GroupId,
                UserId = userId,
                Role = RoleType.Member
            };

            await _unitOfWork
                .GetRepository<GroupMembers, int>()
                .AddAsync(member);

            invitation.UsedCount++;

            await _unitOfWork.SaveChangesAsync();

            return new AcceptInvitationResponseDTO
            {
                Success = true,
                Message = "Joined successfully."
            };
        }

        

        

        public async Task<PagedResult<InvitationResultDTO>> GetGroupInvitationsAsync(int groupId, string userId, int page, int pageSize)
        {
            if (!_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();
            var spec = new GetGroupInvitationsSpecification(groupId, page, pageSize);

            var invitations = await repo.GetAllAsync(spec);
            var totalCount = await repo.CountAsync(i => i.GroupId == groupId);

            var mappedInvitations = _mapper.Map<List<InvitationResultDTO>>(invitations);

            return new PagedResult<InvitationResultDTO>
            {
                Items = mappedInvitations,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (page * pageSize) < totalCount
            };
        }





        public async Task<bool> RevokeInvitationAsync(int invitationId, string userId)
        {
            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var invitation = await repo.GetByIdAsync(invitationId)
                ?? throw new InvitationNotFoundException(invitationId);

            if (!_relationService.IsAdmin(invitation.GroupId) && !_relationService.IsOwner(invitation.GroupId))
                throw new ForbiddenActionException();

            invitation.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync();
            return true;

        }




        public async Task<bool> RevokeAllGroupInvitationsAsync(int groupId, string userId)
        {
            if (!_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<GroupInvitation, int>();

            var spec = new GroupInvitationsByStatusSpecification(groupId, isRevoked: false);
            var activeInvitations = await repo.GetAllAsync(spec);

            if (!activeInvitations.Any())
                return true;

            foreach (var invitation in activeInvitations)
            {
                invitation.IsRevoked = true;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
