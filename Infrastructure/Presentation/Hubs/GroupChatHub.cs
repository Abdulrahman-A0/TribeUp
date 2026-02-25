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
    public class GroupChatHub(IUserGroupRelationService relationService) : Hub
    {
        public async Task JoinGroupChat(int groupId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            await relationService.InitializeAsync(userId!);

            if (!relationService.IsMember(groupId))
                throw new HubException("You must be a member of this group to join its chat.");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"group:{groupId}");
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
