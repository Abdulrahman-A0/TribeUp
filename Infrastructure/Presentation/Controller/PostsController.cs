using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.Posts;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controller
{
    public class PostsController(IServiceManager service) : ApiController
    {
        [HttpPost("create")]
        public async Task<ActionResult> CreatePost(CreatePostDTO dto)
        {
            try
            {
                await service.PostService.CreatePostAsync(dto, User.Identity!.Name!);
                return Ok(new { message = "Post created successfully." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("feed")]
        public async Task<ActionResult> GetFeed()
        {
            var feed = await service.PostService.GetFeedAsync();

            return Ok(feed);
        }

        [HttpPost("{id}/like")]
        public async Task<ActionResult> LikePost(int id)
        {
            await service.PostService.LikePostAsync(id, User.Identity!.Name!);

            return Ok();
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult> Comment(int id, CreateCommentDTO dto)
        {
            await service.PostService.AddCommentAsync(id, dto, User.Identity!.Name!);

            return Ok();
        }
    }
}
