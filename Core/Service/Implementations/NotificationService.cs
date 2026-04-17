using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Users;
using Domain.Exceptions.NotificationExceptions;
using Microsoft.AspNetCore.Identity;
using Service.Specifications.NotificationSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.NotificationModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper,
        INotificationPublisher publisher,
        UserManager<ApplicationUser> userManager,
        IMediaUrlService mediaUrlService) : INotificationService
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

            var dtos = mapper.Map<List<NotificationResponseDTO>>(notifications);

            foreach (var dto in dtos)
            {
                var originalNotif = notifications.FirstOrDefault(n => n.Id == dto.Id);

                if (originalNotif != null && !string.IsNullOrEmpty(originalNotif.ActorUserId))
                {

                    var actorUser = await userManager.FindByIdAsync(originalNotif.ActorUserId);

                    if (actorUser != null)
                    {
                        dto.ActorUserName = actorUser.UserName;
                        dto.ActorPicture = mediaUrlService.BuildUrl(actorUser.ProfilePicture, MediaType.UserProfile);
                    }
                }
            }

            return new PagedNotificationsDTO(dtos, unreadCount);
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


        public async Task CreateRangeAsync(IEnumerable<CreateNotificationDTO> dtos)
        {
            var dtoList = dtos.ToList();
            var notifications = mapper.Map<List<Notification>>(dtoList);

            await unitOfWork.GetRepository<Notification, int>().AddRangeAsync(notifications);
            await unitOfWork.SaveChangesAsync();

            var publishTasks = notifications.Select((n, index) =>
            {
                var responseDto = mapper.Map<NotificationResponseDTO>(n);

                var recipientId = dtoList[index].RecipientUserId;

                return publisher.PublishAsync(recipientId, responseDto);
            });

            await Task.WhenAll(publishTasks);
        }
    }
}
