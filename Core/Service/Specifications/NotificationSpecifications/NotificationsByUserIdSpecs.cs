using Domain.Entities.Users;

namespace Service.Specifications.NotificationSpecifications
{
    internal class NotificationsByUserIdSpecs : BaseSpecifications<Notification, int>
    {
        public NotificationsByUserIdSpecs(string userId, int pageIndex, int pageSize)
            : base(n => n.UserId == userId)
        {
            AddOrderByDescending(n => n.CreateAt);
            ApplyPagination(pageIndex, pageSize);
        }
    }
}
