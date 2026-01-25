using Shared.DTOs.NotificationModule;
using Shared.Enums;

namespace ServiceAbstraction.Contracts
{
    public interface INotificationService
    {
        Task CreateAsync(CreateNotificationDTO dto);

        Task<PagedNotificationsDTO> GetMyNotificationsAsync(string userId, int pageNumber, int pageSize);

        Task MarkAsReadAsync(int notificationId, string userId);
    }

}
