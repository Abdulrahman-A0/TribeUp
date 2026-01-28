using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceAbstraction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Hubs
{
    [Authorize]
    public class GroupChatHub(IGroupChatPermissionService groupChatPermissionService) : Hub
    {
        public async Task JoinGroupChat(int groupId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new HubException("Unauthorized");

            await groupChatPermissionService.EnsureUserCanChatAsync(groupId, userId);

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetGroupName(groupId)
            );
        }

        public async Task LeaveGroupChat(int groupId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                GetGroupName(groupId)
            );
        }

        private static string GetGroupName(int groupId)
            => $"group:{groupId}";
    }

}
