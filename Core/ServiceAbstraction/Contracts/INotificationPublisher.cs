using Shared.DTOs.NotificationModule;

namespace ServiceAbstraction.Contracts
{
    public interface INotificationPublisher
    {
        Task PublishAsync(string userId, NotificationResponseDTO notification);
    }

}
