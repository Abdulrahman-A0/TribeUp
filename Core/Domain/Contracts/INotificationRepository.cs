using Domain.Entities.Users;

namespace Domain.Contracts
{
    public interface INotificationRepository : IGenericRepository<Notification, int>
    {
        Task<int> MarkAllAsReadAsync(string userId);
    }
}
