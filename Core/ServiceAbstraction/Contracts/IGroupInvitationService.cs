using Shared.DTOs.GroupInvitationModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupInvitationService
    {
        Task<InvitationResultDTO> CreateInvitationAsync(int groupId, string userId, CreateInvitationDTO dto);

        Task<List<InvitationResultDTO>> GetGroupInvitationsAsync(int groupId, string userId);

        Task<bool> RevokeInvitationAsync(int invitationId, string userId);

        Task<AcceptInvitationResponseDTO> AcceptInvitationAsync(string token, string userId);
    }

}
