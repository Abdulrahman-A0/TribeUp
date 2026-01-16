using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.Posts;

namespace Presentation.Controller
{
    public class PostsController(IServiceManager service) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDTO dto)
        {
            await service.PostService.CreatePostAsync(dto, User.Identity!.Name!);

            return Ok();
        }

        [HttpDelete("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var feed = await service.PostService.GetFeedAsync();

            return Ok(feed);
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePost(int id)
        {
            await service.PostService.LikePostAsync(id, User.Identity!.Name!);

            return Ok();
        }

        [HttpPost("{id}/comment")]
        public async Task<IActionResult> Comment(int id, CreateCommentDTO dto)
        {
            await service.PostService.AddCommentAsync(id, dto, User.Identity!.Name!);

            return Ok();
        }
    }
}
