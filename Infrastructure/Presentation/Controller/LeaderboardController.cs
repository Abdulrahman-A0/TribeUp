using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;

namespace Presentation.Controller
{
    [Authorize]
    public class LeaderboardController(IServiceManager service) : ApiController
    {
        /// <summary>
        /// Retrieves the top ranked groups in the leaderboard based on their total score.
        /// </summary>
        /// <param name="top">
        /// Number of groups to return. Must be between 1 and 100.
        /// </param>
        /// <returns>
        /// A collection of leaderboard entries containing group information,
        /// rank, score, badge details, and profile picture URL.
        /// </returns>
        /// <response code="200">
        /// Leaderboard retrieved successfully.
        /// </response>
        /// <response code="400">
        /// The provided top value is outside the allowed range.
        /// </response>
        /// <response code="500">
        /// An unexpected server error occurred.
        /// </response>
        
        [HttpGet]
        public async Task<IActionResult> GetLeaderboard(
        [FromQuery] int top = 5)
            => Ok(await service.LeaderboardService.GetTopGroupsAsync(top));

    }
}
