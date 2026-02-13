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
        public Task<List<GroupMemberResultDTO>> GetGroupMembersAsync(int groupId, string userId);
        
        Task<bool> LeaveGroupAsync(int groupId, string userId);

        public Task<bool> PromoteToAdminAsync(int groupId, string actorUserId, int groupMemberId);
        public Task<bool> DemoteAdminAsync(int groupId, string actorUserId, int groupMemberId);
        public Task<bool> KickMemberAsync(int groupId, string actorUserId, int groupMemberId);

    }
}
