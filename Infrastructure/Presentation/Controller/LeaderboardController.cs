using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;

namespace Presentation.Controller
{
    [Authorize]
    public class LeaderboardController(IServiceManager service) : ApiController
    {

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard(
        [FromQuery] int top = 5)
            => Ok(await service.LeaderboardService.GetTopGroupsAsync(top));

    }
}
