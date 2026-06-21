using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.DTOs.VirtualRoomModule;

namespace Presentation.Hubs
{
    [Authorize]
    public class VirtualRoomHub : Hub
    {
        public async Task JoinVirtualRoom(int groupId, ParticipantDTO player)
        {
            var groupName = GetRoomName(groupId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.OthersInGroup(groupName).SendAsync("PlayerJoined", player);
        }

        public async Task LeaveVirtualRoom(int groupId, string userId)
        {
            var groupName = GetRoomName(groupId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.OthersInGroup(groupName).SendAsync("PlayerLeft", userId);
        }

        public async Task Move(int groupId, string userId, float[] position, float rotationY)
        {
            await Clients.OthersInGroup(GetRoomName(groupId)).SendAsync("PlayerMoved", userId, position, rotationY);
        }

        private static string GetRoomName(int groupId) => $"vr_room_{groupId}";
    }
}
