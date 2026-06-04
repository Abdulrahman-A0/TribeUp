using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class GroupsController(IServiceManager serviceManager) : ApiController
    {
        

        [HttpGet("MyGroups")]
        public async Task<ActionResult<List<GroupResultDTO>>> GetMyGroupsAsync(int page = 1, int pageSize = 10)
            => Ok(await serviceManager.GroupService.GetMyGroupsAsync(page, pageSize,UserId));


        [HttpGet("GetGroup/{Id:int}")]
        public async Task<ActionResult<GroupDetailsResultDTO>> GetGroupByIdAsync(int Id)
            => Ok(await serviceManager.GroupService.GetGroupByIdAsync(Id));


        [HttpPost("CreateGroup")]

        public async Task<ActionResult<GroupResultDTO>> CreateGroupAsync(CreateGroupDTO createGroupDTO)
            => Ok(await serviceManager.GroupService.CreateGroupAsync(createGroupDTO, UserId));


        [HttpPut("UpdateGroup/{Id:int}")]
        public async Task<ActionResult<GroupResultDTO>> UpdateGroupAsync(int Id, [FromBody] UpdateGroupDTO updateGroupDTO)
            => Ok(await serviceManager.GroupService.UpdateGroupAsync(Id, updateGroupDTO, UserId));


        [HttpDelete("DeleteGroup/{Id:int}")]
        public async Task<ActionResult> DeleteGroupAsync(int Id)
            => Ok(await serviceManager.GroupService.DeleteGroupAsync(Id, UserId));


        [HttpPut("UpdateGroupPicture/{Id:int}")]
        //[Consumes("multipart/form-data")]
        public async Task<ActionResult<GroupResultDTO>> UpdateGroupPictureAsync(int Id, [FromForm] UpdateGroupPictureDTO updateGroupPictureDTO)
            => Ok(await serviceManager.GroupService.UpdateGroupPictureAsync(Id, updateGroupPictureDTO, UserId));


        [HttpDelete("DeleteGroupPicture/{Id:int}")]
        public async Task<ActionResult> DeleteGroupPictureAsync(int Id)
            => Ok(await serviceManager.GroupService.DeleteGroupPictureAsync(Id, UserId));


        [HttpGet("ExploreGroups")]
        public async Task<ActionResult<PagedResult<GroupResultDTO>>> ExploreGroupsAsync(int page = 1, int pageSize = 20, string? search = null)
            => Ok(await serviceManager.GroupService.ExploreGroupsAsync(page, pageSize, UserId, search));




        /// <summary>
        /// Retrieves all groups that the current user is following.
        /// </summary>
        /// <remarks>
        /// This endpoint is designed specifically for the "Followed" tab in the UI.
        /// It returns only the groups where the user is a follower (not a joined member).
        /// </remarks>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The number of items per page (default is 10).</param>
        /// <response code="200">Returns the paginated list of followed groups.</response>
        [HttpGet("FollowedGroups")]
        public async Task<ActionResult<PagedResult<GroupResultDTO>>> GetFollowedGroups(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await serviceManager.GroupService.GetFollowedGroupsAsync(page, pageSize, UserId);
            return Ok(result);
        }

    }
}
