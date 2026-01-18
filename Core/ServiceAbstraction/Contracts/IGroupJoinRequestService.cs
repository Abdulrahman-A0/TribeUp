using Shared.DTOs.GroupJoinRequestModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupJoinRequestService
    {
        // User requests to join a private group
        Task<GroupJoinRequestResultDTO> CreateJoinRequestAsync(int groupId, string userId);

        // Admin gets all pending requests for a group
        Task<List<GroupJoinRequestResultDTO>> GetPendingRequestsAsync(int groupId, string userId);

        // Admin approves a join request
        Task<bool> ApproveJoinRequestAsync(int requestId, string userId);

        // Admin rejects a join request
        Task<bool> RejectJoinRequestAsync(int requestId, string userId);

        // NEW methods to use all specs:
        Task<GroupJoinRequestResultDTO> GetRequestByIdAsync(int requestId, string userId);
        Task<List<GroupJoinRequestResultDTO>> GetUserRequestsAsync(string userId);
        Task<GroupJoinRequestResultDTO?> GetUserRequestForGroupAsync(int groupId, string userId);

    }
}
