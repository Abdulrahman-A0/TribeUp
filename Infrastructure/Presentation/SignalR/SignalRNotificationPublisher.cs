using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;

namespace Presentation.SignalR
{
    public class SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext) : INotificationPublisher
    {
        public async Task PublishAsync(string userId, NotificationResponseDTO notification)
        {
            await hubContext.Clients
            .Group(userId)
            .SendAsync("notification-received", notification);
        }
    }
}
