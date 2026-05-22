using Shared.DTOs.GroupInvitationModule;
using Shared.DTOs.Posts;
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

        Task<InvitationResultDTO?> GetActiveInvitationAsync(int groupId, string userId);

        Task<bool> RevokeInvitationAsync(int invitationId, string userId);

        Task<AcceptInvitationResponseDTO> AcceptInvitationAsync(string token, string userId);

    }

}
