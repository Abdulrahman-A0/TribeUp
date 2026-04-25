using Shared.DTOs.StoriesModule;

namespace ServiceAbstraction.Contracts
{
    public interface IStoryService
    {
        Task<StoryResponseDTO> CreateStoryAsync(CreateStoryDTO dto, string currentUserId);
        Task<IEnumerable<StoryResponseDTO>> GetActiveGroupStoriesAsync(int groupId, string currentUserId);
        Task<IEnumerable<StoryFeedItemDTO>> GetStoryFeedAsync(string currentUserId);
        Task MarkStoryAsViewedAsync(int storyId, string currentUserId);
        Task DeleteStoryAsync(int storyId, string currentUserId);
    }
}
