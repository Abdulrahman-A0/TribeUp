using Shared.DTOs.GroupMemberModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupAuthorizationService
    {
        Task<GroupMemberResultDTO> EnsureUserCanChatAsync(int groupId, string userId);
        Task<GroupMemberResultDTO> EnsureUserIsAdminAsync(int groupId, string userId);
        Task<GroupMemberResultDTO> EnsureUserIsOwnerAsync(int groupId, string userId);
        Task<GroupMemberResultDTO> EnsureUserIsOwnerOrAdminAsync(int groupId, string userId);
    }
}
