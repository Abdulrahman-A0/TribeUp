using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class GroupInvitationsController(IServiceManager serviceManager)
    : ApiController
    {
        [HttpPost("CreateInvitations/{groupId:int}")]
        public async Task<ActionResult<InvitationResultDTO>>Create(int groupId, CreateInvitationDTO dto)
            => Ok(await serviceManager.GroupInvitationService.CreateInvitationAsync(groupId, UserId, dto));


        [HttpPost("AcceptInvitations/{token}")]
        public async Task<ActionResult<AcceptInvitationResponseDTO>>Accept(string token)
            => Ok(await serviceManager.GroupInvitationService.AcceptInvitationAsync(token, UserId));


        [HttpGet("GetActiveInvitation/{groupId:int}")]
        public async Task<ActionResult<InvitationResultDTO>> GetActiveInvitation(int groupId)
            => Ok(await serviceManager.GroupInvitationService.GetActiveInvitationAsync(groupId, UserId));


        [HttpDelete("RevokeInvitation/{invitationId:int}")]
        public async Task<ActionResult<bool>> Revoke(int invitationId)
            => Ok(await serviceManager.GroupInvitationService.RevokeInvitationAsync(invitationId, UserId));

    }

}
