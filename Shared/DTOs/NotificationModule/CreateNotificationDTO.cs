using Shared.Enums;

namespace Shared.DTOs.NotificationModule
{
    public record CreateNotificationDTO
    {
        public string RecipientUserId { get; init; }
        public string ActorUserId { get; init; }
        public NotificationType Type { get; init; }
        public string Title { get; init; }
        public string Message { get; init; }
        public int? ReferenceId { get; init; }
    }
}
