using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceAbstraction.Contracts;
using Shared.DTOs.Posts;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controller
{
    [Authorize]
    public class PostsController(IServiceManager service) : ApiController
    {
        [HttpPost("CreatePost")]
        public async Task<ActionResult> CreatePost(CreatePostDTO dto)
            => Ok(await service.PostService.CreatePostAsync(dto, UserId));
        

        [HttpGet("Feed")]
        public async Task<ActionResult> GetFeed(int page = 1, int pageSize = 20)
            =>Ok(await service.PostService.GetFeedAsync(UserId, page, pageSize));


        [HttpGet("{groupId}/GroupFeed")]
        public async Task<ActionResult> GetGroupFeed(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetGroupFeedAsync(UserId, groupId, page, pageSize));


        [HttpPost("{postId:int}/AddLike")]
        public async Task<ActionResult> LikePost(int postId)
            => Ok(await service.PostService.LikePostAsync(postId, UserId));

        [HttpGet("{postId:int}/Likes")]
        public async Task<ActionResult> GetLikes(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetLikesByPostIdAsync(postId, page, pageSize));


        [HttpPost("{postId:int}/AddComment")]
        public async Task<ActionResult> Comment(int postId, CreateCommentDTO dto)
            => Ok(await service.PostService.AddCommentAsync(postId, dto, UserId));


        [HttpGet("{postId:int}/Comments")]
        public async Task<ActionResult> GetComments(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetCommentsByPostIdAsync(postId, page, pageSize));

    }
}
