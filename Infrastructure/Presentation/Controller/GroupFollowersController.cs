using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupFollowerModule;
using Shared.DTOs.Posts;
using System.Security.Claims;

namespace Presentation.Controller
{
    [Authorize]
    [Route("api/groups/{groupId:int}/GetFollowers")]
    public class GroupFollowersController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<GroupFollowerResultDTO>>> GetFollowers(int groupId, int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var result = await serviceManager.GroupFollowerService
                .GetGroupFollowersAsync(groupId, page, pageSize, searchTerm);

            return Ok(result);
        }

        
        [HttpPost("ToggleFollow")]
        public async Task<ActionResult<FollowActionResponseDTO>> ToggleFollow(int groupId)
        {
            var result = await serviceManager.GroupFollowerService
                .ToggleFollowAsync(groupId, UserId);

            return Ok(result);
        }


        [HttpDelete("{followerId}")]
        public async Task<ActionResult<bool>> RemoveFollower(int groupId, int followerId)
        {

            await serviceManager.GroupFollowerService
                .RemoveFollowerAsync(groupId, followerId, UserId);

            return NoContent();
        }
    }
}