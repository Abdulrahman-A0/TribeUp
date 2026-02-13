using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupInvitationModule;
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
        [HttpPost("Groups/{groupId:int}/Invitations")]
        public async Task<ActionResult<InvitationResultDTO>>Create(int groupId, CreateInvitationDTO dto)
            => Ok(await serviceManager.GroupInvitationService.CreateInvitationAsync(groupId, UserId, dto));


        [HttpPost("Invitations/{token}/Accept")]
        public async Task<ActionResult<AcceptInvitationResponseDTO>>Accept(string token)
            => Ok(await serviceManager.GroupInvitationService.AcceptInvitationAsync(token, UserId));
    }

}
