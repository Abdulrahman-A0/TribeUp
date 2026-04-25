namespace Shared.DTOs.StoriesModule
{
    public record StoryResponseDTO
    {
        public int Id { get; init; }
        public string? Caption { get; init; }
        public string? MediaURL { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public int ViewsCount { get; init; }

        // Creator info
        public string CreatorId { get; init; }
        public string CreatorUserName { get; init; }
        public string? GroupProfilePicture { get; init; }

        public bool IsViewedByCurrentUser { get; init; }
    }
}
