using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controller;
using ServiceAbstraction.Contracts;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/groups/{groupId}/virtual-room")]
    public class VirtualRoomController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom(int groupId)
        {
            var result = await serviceManager.VirtualRoomService.JoinRoomAsync(groupId, UserId);
            return Ok(result);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveRoom(int groupId)
        {
            await serviceManager.VirtualRoomService.LeaveRoomAsync(groupId, UserId);
            return NoContent();
        }

        [HttpGet("participants")]
        public async Task<IActionResult> GetParticipants(int groupId)
        {
            var participants = await serviceManager.VirtualRoomService.GetActiveParticipantsAsync(groupId);
            return Ok(participants);
        }
    }
}