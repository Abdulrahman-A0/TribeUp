using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.StoriesModule;

namespace Presentation.Controller
{
    [Authorize]
    public class StoryController(IServiceManager serviceManager) : ApiController
    {
        [HttpPost("CreateStory")]
        public async Task<ActionResult<StoryResponseDTO>> CreateStoryAsync(CreateStoryDTO dto)
            => Ok(await serviceManager.StoryService.CreateStoryAsync(dto, UserId));

        [HttpDelete("DeleteStory/{storyId}")]
        public async Task<IActionResult> DeleteStoryAsync(int storyId)
        {
            await serviceManager.StoryService.DeleteStoryAsync(storyId, UserId);
            return NoContent();
        }

        [HttpGet("GetGroupStories/{groupId}")]
        public async Task<ActionResult<IEnumerable<StoryResponseDTO>>> GetGroupStoriesAsync(int groupId)
            => Ok(await serviceManager.StoryService.GetActiveGroupStoriesAsync(groupId, UserId));

        [HttpGet("GetStoryFeed")]
        public async Task<ActionResult<IEnumerable<StoryFeedItemDTO>>> GetStoryFeedAsync(int pageNumber = 1, int pageSize = 10)
            => Ok(await serviceManager.StoryService.GetStoryFeedAsync(UserId, pageNumber, pageSize));

        [HttpPut("MarkAsViewed/{storyId}")]
        public async Task<IActionResult> MarkAsViewedAsync(int storyId)
        {
            await serviceManager.StoryService.MarkStoryAsViewedAsync(storyId, UserId);
            return NoContent();
        }
    }
}
