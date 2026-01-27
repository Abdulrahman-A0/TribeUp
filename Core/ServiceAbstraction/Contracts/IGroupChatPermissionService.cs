using Shared.DTOs.GroupMemberModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupChatPermissionService
    {
        Task<GroupMemberResultDTO> EnsureUserCanChatAsync(int groupId, string userId);
    }
}
