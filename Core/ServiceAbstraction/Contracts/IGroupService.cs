using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.GroupModule;
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
    }
}
