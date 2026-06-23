using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controller;
using ServiceAbstraction.Contracts;
using Shared.DTOs.VirtualRoomModule;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/groups/{groupId}/virtual-room")]
    public class VirtualRoomController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("join")]
        public async Task<ActionResult<RoomDetailsDTO>> JoinRoom(int groupId)
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
        public async Task<ActionResult<IEnumerable<ParticipantDTO>>> GetParticipants(int groupId)
        {
            var participants = await serviceManager.VirtualRoomService.GetActiveParticipantsAsync(groupId);
            return Ok(participants);
        }

        [HttpGet("voice-token")]
        public ActionResult<LiveKitTokenDTO> GetVoiceToken(int groupId)
        {
            var result = serviceManager.VoiceChatService.GenerateToken(groupId, UserId, UserName);
            return Ok(result);
        }
    }
}