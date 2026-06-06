using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMessageModule;
using Shared.DTOs.GroupMessages;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class GroupChatController(IServiceManager serviceManager) : ApiController
    {
        [HttpGet("GetMessages")]
        public async Task<ActionResult<PagedResult<GroupMessageResponseDTO>>> GetMessagesAsync(int groupId, int page = 1, int pageSize = 20)
            => Ok( await serviceManager.GroupChatService.GetMessagesAsync(groupId, page, pageSize, UserId));


        [HttpPost("SendMessage/{groupId}")]
        public async Task<ActionResult<GroupMessageResponseDTO>> SendMessageAsync(int groupId, [FromBody] SendGroupMessageDTO sendGroupMessageDTO)
            => Ok(await serviceManager.GroupChatService.SendMessageAsync(groupId, sendGroupMessageDTO, UserId));

        [HttpGet("ChatInbox")]
        public async Task<ActionResult<List<GroupChatInboxDTO>>> GetChatInboxAsync()
            => Ok(await serviceManager.GroupChatService.GetChatInboxAsync(UserId));




        /// <summary>
        /// Edits an existing group chat message.
        /// </summary>
        /// <remarks>
        /// Only the original sender of the message is allowed to edit its content.
        /// 
        /// When a message is successfully edited:
        /// - The database is updated.
        /// - A SignalR event ("ReceiveMessageEdit") is automatically broadcasted to all active members in the group chat room to update the UI in real-time.
        /// 
        /// Editable fields:
        /// - Content
        /// 
        /// Example:
        /// 
        /// {
        ///   "content": "Updated message text here..."
        /// }
        /// </remarks>
        /// <param name="messageId">The unique identifier of the message to edit.</param>
        /// <param name="dto">The updated content of the message.</param>
        /// <response code="200">Message updated successfully. Returns the updated message details.</response>
        /// <response code="403">The current user is not the sender of this message.</response>
        /// <response code="404">The message was not found or has been deleted.</response>

        [HttpPut("{messageId:long}/EditMessage")]
        public async Task<ActionResult<EditedMessageResponseDTO>> EditMessage(
            long messageId,
            [FromBody] EditGroupMessageDTO dto)
            => Ok(await serviceManager.GroupChatService.EditMessageAsync(messageId, dto, UserId));


        /// <summary>
        /// Deletes a specific group chat message.
        /// </summary>
        /// <remarks>
        /// This endpoint performs a **Soft Delete** on the message to preserve data integrity.
        /// 
        /// Permissions:
        /// Only the following users can delete a message:
        /// - The original sender of the message.
        /// - The Group Admin.
        /// - The Group Owner.
        /// 
        /// When a message is deleted:
        /// - It is marked as deleted in the database (IsDeleted = true).
        /// - A SignalR event ("ReceiveMessageDeletion") containing the messageId is broadcasted to the group room so the front-end can remove the bubble from the chat interface.
        /// </remarks>
        /// <param name="messageId">The unique identifier of the message to delete.</param>
        /// <response code="200">Message deleted successfully.</response>
        /// <response code="403">User lacks permission to delete this message.</response>
        /// <response code="404">The message was not found or is already deleted.</response>

        [HttpDelete("{messageId:long}/DeleteMessage")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            await serviceManager.GroupChatService.DeleteMessageAsync(messageId, UserId);

            return Ok(new { Message = "Message deleted successfully." });
        }

    }
}
