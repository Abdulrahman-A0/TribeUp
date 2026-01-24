using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.GroupModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupService
    {
        Task<List<GroupResultDTO>> GetAllGroupsAsync();
        Task<GroupDetailsResultDTO> GetGroupByIdAsync(int groupId);
        Task<GroupResultDTO> CreateGroupAsync(CreateGroupDTO createGroupDTO, string userId);
        Task<GroupResultDTO> UpdateGroupAsync(int Id, UpdateGroupDTO updateGroupDTO, string userId);
        Task<bool> DeleteGroupAsync(int groupId, string userId);
        Task<GroupResultDTO> UpdateGroupPictureAsync(int groupId, UpdateGroupPictureDTO updateGroupPictureDTO, string userId);
        Task<bool> DeleteGroupPictureAsync(int groupId, string userId);
        Task<List<GroupResultDTO>> GetMyGroupsAsync(string userId);
        Task<PagedResult<GroupResultDTO>> ExploreGroupsAsync(int page, int pageSize, string userId);
    }
}
