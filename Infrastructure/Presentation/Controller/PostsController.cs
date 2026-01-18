using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public async Task<ActionResult> GetFeed(int page = 1, int pageSize = 20)
        {
            var feed = await service.PostService.GetFeedAsync(User.Identity!.Name!, page, pageSize);

            return Ok(feed);
        }

        [HttpPost("{id}/like")]
        public async Task<ActionResult> LikePost(int id)
        {
            bool isLiked = await service.PostService.LikePostAsync(id, User.Identity!.Name!);

            return Ok(new
            {
                liked = isLiked,
                message = isLiked
            ? "Post liked"
            : "Like removed"
            });
        }

        [HttpPost("{id}/comment")]
        public async Task<ActionResult> Comment(int id, CreateCommentDTO dto)
        {
            await service.PostService.AddCommentAsync(id, dto, User.Identity!.Name!);

            return Ok();
        }
    }
}
