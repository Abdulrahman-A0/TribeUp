using Shared.DTOs.GroupMemberModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupMemberService
    {
        Task<JoinGroupResponseDTO> JoinOrRequestGroupAsync(int groupId, string userId);
        Task<bool> LeaveGroupAsync(int groupId, string userId);
    }
}
