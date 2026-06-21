using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMessageModule;
using Shared.DTOs.GroupMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.SignalR
{
    public class SignalRGroupChatNotifier(IHubContext<GroupChatHub> hubContext) : IGroupChatNotifier
    {
        public async Task NotifyGroupAsync(int groupId, GroupMessageResponseDTO message)
        {
            await hubContext.Clients
                .Group($"group:{groupId}")
                .SendAsync("ReceiveGroupMessage", message);

            var inboxUpdate = new InboxUpdateEventDTO
            {
                GroupId = groupId,
                LastMessage = message.Content,
                LastMessageSenderName = message.SenderName,
                SentAt = message.SentAt
            };

            await hubContext.Clients
                .Group($"group:{groupId}")
                .SendAsync("UpdateInbox", inboxUpdate);
        }

        public async Task NotifyMessageEditedAsync(int groupId, EditedMessageResponseDTO editedMessage)
        {
            await hubContext.Clients
                .Group($"group:{groupId}")
                .SendAsync("ReceiveMessageEdit", editedMessage);
        }

        public async Task NotifyMessageDeletedAsync(int groupId, long messageId)
        {
            await hubContext.Clients
                .Group($"group:{groupId}")
                .SendAsync("ReceiveMessageDeletion", messageId);
        }
    }
}
