namespace Shared.DTOs.NotificationModule
{
    public record PagedNotificationsDTO(
        IEnumerable<NotificationResponseDTO> Notifications,
        int UnreadCount);

}
