using Shared.DTOs.PollModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IPollService
    {
        Task<PollResultDTO> CreatePollAsync(int groupId, string userId, CreatePollDTO dto);
        Task<ToggleVoteResultDTO> ToggleVoteAsync(int pollId, int optionId, string userId);
        Task<PollResultDTO> GetPollResultsAsync(int pollId, string userId);
        Task<PagedResult<PollResultDTO>> GetGroupPollsAsync(int groupId, string userId, int page, int pageSize);
        Task<bool> DeletePollAsync(int pollId, string userId);
        Task<PollResultDTO> EditPollAsync(int pollId, string userId, EditPollDTO dto);
    }
}
