using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupJoinRequestModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class GroupJoinRequestsController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet("PendingRequests/Group/{groupId:int}")]
        public async Task<ActionResult<List<GroupJoinRequestResultDTO>>> GetPendingRequestsAsync(int groupId)
            => Ok(await serviceManager.GroupJoinRequestService.GetPendingRequestsAsync(groupId, UserId));

        [HttpGet("Request/{requestId:int}")]
        public async Task<ActionResult<GroupJoinRequestResultDTO>> GetRequestByIdAsync(int requestId)
            => Ok(await serviceManager.GroupJoinRequestService.GetRequestByIdAsync(requestId, UserId));

        [HttpPost("Approve/Request/{requestId:int}")]
        public async Task<ActionResult<bool>> ApproveJoinRequestAsync(int requestId)
            => Ok(await serviceManager.GroupJoinRequestService.ApproveJoinRequestAsync(requestId, UserId));

        [HttpPost("Reject/Request/{requestId:int}")]
        public async Task<ActionResult<bool>> RejectJoinRequestAsync(int requestId)
            => Ok(await serviceManager.GroupJoinRequestService.RejectJoinRequestAsync(requestId, UserId));




        [HttpGet("User/Requests")]
        public async Task<ActionResult<List<GroupJoinRequestResultDTO>>> GetUserRequestsAsync()
            => Ok(await serviceManager.GroupJoinRequestService.GetUserRequestsAsync(UserId));

        [HttpGet("User/Group/{groupId:int}")]
        public async Task<ActionResult<GroupJoinRequestResultDTO>> GetMyRequestForGroup(int groupId)
            => Ok(await serviceManager.GroupJoinRequestService.GetUserRequestForGroupAsync(groupId, UserId));
    }
}
