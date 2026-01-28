using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMessages;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    public class GroupChatController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet("GetMessages")]
        public async Task<ActionResult<PagedResult<GroupMessageResponseDTO>>> GetMessagesAsync(int groupId, int page = 1, int pageSize = 20)
            => Ok( await serviceManager.GroupChatService.GetMessagesAsync(groupId, page, pageSize, UserId));


        [HttpPost("SendMessage/{groupId}")]
        public async Task<ActionResult<GroupMessageResponseDTO>> SendMessageAsync(int groupId, [FromBody] SendGroupMessageDTO sendGroupMessageDTO)
            => Ok(await serviceManager.GroupChatService.SendMessageAsync(groupId, sendGroupMessageDTO, UserId));

    }
}
