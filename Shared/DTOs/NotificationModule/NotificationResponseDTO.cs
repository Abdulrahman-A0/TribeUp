namespace Shared.DTOs.NotificationModule
{
    public record NotificationResponseDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Message { get; init; }

        public string Type { get; init; }

        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }

        public int? ReferenceId { get; init; }
    }
}
