using Shared.DTOs.LeaderboardModule;

namespace ServiceAbstraction.Contracts;

public interface ILeaderboardService
{
    Task<IEnumerable<LeaderboardGroupDTO>> GetTopGroupsAsync(int top);
}