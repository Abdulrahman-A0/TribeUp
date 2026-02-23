using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
using Shared.DTOs.GroupMemberModule;
using Shared.DTOs.Posts;
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

        [HttpGet("GroupMembers/{groupId:int}")]
        public async Task<ActionResult<PagedResult<GroupMemberResultDTO>>> GetGroupMembersAsync(int groupId, int page = 1, int pageSize = 10, string? search = null)
            => Ok(await serviceManager.GroupMemberService.GetGroupMembersAsync(groupId, UserId, page, pageSize, search));




        [HttpPost("LeaveGroup/{groupId:int}")]
        public async Task<ActionResult<bool>> LeaveGroupAsync(int groupId)
            => Ok(await serviceManager.GroupMemberService.LeaveGroupAsync(groupId, UserId));


        [HttpPost("Promote/{groupId:int}/User/{GroupMemberId:int}")]
        public async Task<ActionResult<bool>> PromoteToAdminAsync(int groupId, int GroupMemberId)
            => Ok(await serviceManager.GroupMemberService.PromoteToAdminAsync(groupId, UserId, GroupMemberId));


        [HttpPost("Demote/{groupId:int}/User/{GroupMemberId:int}")]
        public async Task<ActionResult<bool>> DemoteAdminAsync(int groupId, int GroupMemberId)
            => Ok(await serviceManager.GroupMemberService.DemoteAdminAsync(groupId, UserId, GroupMemberId));


        [HttpPost("Kick/{groupId:int}/User/{GroupMemberId:int}")]
        public async Task<ActionResult<bool>> KickMemberAsync(int groupId, int GroupMemberId)
            => Ok(await serviceManager.GroupMemberService.KickMemberAsync(groupId, UserId, GroupMemberId));
    }
}
