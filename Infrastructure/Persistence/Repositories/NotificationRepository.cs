using Domain.Contracts;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;

namespace Persistence.Repositories
{
    public class NotificationRepository(AppDbContext context)
        : GenericRepository<Notification, int>(context),
          INotificationRepository
    {
        public async Task<int> MarkAllAsReadAsync(string userId)
        {
            return await context.Set<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true));
        }
    }
}
