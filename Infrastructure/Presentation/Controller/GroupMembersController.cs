using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMemberModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class GroupMembersController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("JoinGroup/{groupId:int}")]
        public async Task<ActionResult<JoinGroupResponseDTO>> JoinOrRequestGroupAsync(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await serviceManager.GroupMemberService.JoinOrRequestGroupAsync(groupId, userId));
        }

        [HttpPost("LeaveGroup/{groupId:int}")]
        public async Task<ActionResult<bool>> LeaveGroupAsync(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(await serviceManager.GroupMemberService.LeaveGroupAsync(groupId, userId));
        }
    }
}
