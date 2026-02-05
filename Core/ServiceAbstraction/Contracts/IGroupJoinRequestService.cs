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
        Task<GroupJoinRequestResultDTO> CreateJoinRequestAsync(int groupId, string userId);

        Task<List<GroupJoinRequestResultDTO>> GetPendingRequestsAsync(int groupId, string userId);

        Task<bool> ApproveJoinRequestAsync(int requestId, string userId);

        Task<bool> RejectJoinRequestAsync(int requestId, string userId);

        Task<GroupJoinRequestResultDTO> GetRequestByIdAsync(int requestId, string userId);
        Task<List<GroupJoinRequestResultDTO>> GetUserRequestsAsync(string userId);
        Task<GroupJoinRequestResultDTO?> GetUserRequestForGroupAsync(int groupId, string userId);
        Task<bool> CancelMyJoinRequestAsync(int requestId, string userId);

    }
}
