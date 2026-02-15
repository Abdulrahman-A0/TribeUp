using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using ServiceAbstraction.Contracts;
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

            await hubContext.Clients.Group($"group:{groupId}")
                .SendAsync("UpdateInbox", new
                {
                    GroupId = groupId,
                    LastMessage = message.Content,
                    SentAt = message.SentAt
                });
        }
    }
}
