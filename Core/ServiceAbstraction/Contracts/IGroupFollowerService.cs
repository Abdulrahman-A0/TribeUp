using Shared.DTOs.GroupFollowerModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupFollowerService
    {
        Task<PagedResult<GroupFollowerResultDTO>> GetGroupFollowersAsync(int groupId, int page, int pageSize, string? searchTerm = null);
        public Task<FollowActionResponseDTO> ToggleFollowAsync(int groupId, string userId);
        Task RemoveFollowerAsync(int groupId, int followerId, string userId);
    }
}
