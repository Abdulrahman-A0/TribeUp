using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupModule;
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
        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<List<GroupResultDTO>>> GetAllGroupsAsync()
            => Ok(await serviceManager.GroupService.GetAllGroupsAsync());


        [HttpGet("GetGroup/{Id:int}")]
        public async Task<ActionResult<GroupDetailsResultDTO>> GetGroupByIdAsync(int Id)
            => Ok(await serviceManager.GroupService.GetGroupByIdAsync(Id));

        [HttpPost("CreateGroup")]
        public async Task<ActionResult<GroupResultDTO>> CreateGroupAsync([FromBody] CreateGroupDTO createGroupDTO)
            => Ok(await serviceManager.GroupService.CreateGroupAsync(createGroupDTO, UserId));

        [HttpPut("UpdateGroup/{Id:int}")]
        public async Task<ActionResult<GroupResultDTO>> UpdateGroupAsync(int Id ,[FromBody] UpdateGroupDTO updateGroupDTO)
            => Ok(await serviceManager.GroupService.UpdateGroupAsync(Id, updateGroupDTO, UserId));

        [HttpDelete("DeleteGroup/{Id:int}")]
        public async Task<ActionResult> DeleteGroupAsync(int Id)
            => Ok(await serviceManager.GroupService.DeleteGroupAsync(Id, UserId));
    }
}
