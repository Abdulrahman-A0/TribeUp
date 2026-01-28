using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Users;
using Domain.Exceptions.NotificationExceptions;
using Service.Specifications.NotificationSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;

namespace Service.Implementations
{
    public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper,
        INotificationPublisher publisher) : INotificationService
    {
        public async Task<PagedNotificationsDTO> GetMyNotificationsAsync(string userId, int pageIndex, int pageSize)
        {
            var specs = new NotificationsByUserIdSpecs(userId, pageIndex, pageSize);

            var notifications = await unitOfWork
                .GetRepository<Notification, int>()
                .GetAllAsync(specs);

            var unreadCount = await unitOfWork
                .GetRepository<Notification, int>()
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return new PagedNotificationsDTO
                (mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications), unreadCount);
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {

            var notification = await unitOfWork
                .GetRepository<Notification, int>()
                .GetByIdAsync(notificationId)
                ?? throw new NotificationNotFoundException(notificationId);

            if (notification.UserId != userId)
                throw new NotificationAccessDeniedException();

            notification.IsRead = true;

            await unitOfWork.SaveChangesAsync();
        }

        public async Task CreateAsync(CreateNotificationDTO dto)
        {
            if (dto.RecipientUserId == dto.ActorUserId)
                return;

            var notification = mapper.Map<Notification>(dto);

            await unitOfWork
                .GetRepository<Notification, int>()
                .AddAsync(notification);

            await unitOfWork.SaveChangesAsync();

            var responseDto = mapper.Map<NotificationResponseDTO>(notification);

            await publisher.PublishAsync(dto.RecipientUserId, responseDto);
        }
    }
}
