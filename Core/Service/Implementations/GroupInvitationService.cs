using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.ValidationExceptions;
using Service.Specifications.GroupInvitaionSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserGroupRelationService _relationService;

        public GroupInvitationService(
            IUnitOfWork unitOfWork,
            IUserGroupRelationService relationService)
        {
            _unitOfWork = unitOfWork;
            _relationService = relationService;
        }

        public async Task<InvitationResultDTO> CreateInvitationAsync(
            int groupId,
            string userId,
            CreateInvitationDTO dto)
        {
            if (!_relationService.IsAdmin(groupId) &&
                !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var token = Guid.NewGuid().ToString("N");

            var invitation = new GroupInvitation
            {
                GroupId = groupId,
                Token = token,
                UserId = userId,
                ExpiresAt = dto.ExpiresAt,
                MaxUses = dto.MaxUses
            };

            await _unitOfWork
                .GetRepository<GroupInvitation, int>()
                .AddAsync(invitation);

            await _unitOfWork.SaveChangesAsync();

            return new InvitationResultDTO
            {
                Id = invitation.Id,
                Token = invitation.Token,
                CreatedAt = invitation.CreatedAt,
                ExpiresAt = invitation.ExpiresAt,
                MaxUses = invitation.MaxUses,
                UsedCount = invitation.UsedCount,
                IsRevoked = invitation.IsRevoked
            };
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

        public Task<List<InvitationResultDTO>> GetGroupInvitationsAsync(int groupId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeInvitationAsync(int invitationId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
