namespace Shared.DTOs.StoriesModule
{
    public record StoryFeedItemDTO
    {
        public int GroupId { get; init; }
        public string GroupName { get; init; }
        public string? GroupProfilePicture { get; init; }

        public bool HasUnseenStories { get; init; }
        public DateTime LatestStoryDate { get; init; }
    }
}
