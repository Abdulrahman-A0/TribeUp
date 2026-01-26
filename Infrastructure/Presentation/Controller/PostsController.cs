using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controller
{
    [Authorize]
    public class PostsController(IServiceManager service) : ApiController
    {
        [HttpPost("CreatePost")]
        public async Task<ActionResult<CreatePostResultDTO>> CreatePost(
            [FromForm] CreatePostDTO dto,
            [FromForm] List<IFormFile> mediaFiles)
            => Ok(await service.PostService.CreatePostAsync(dto, UserId, mediaFiles));


        [HttpGet("{postId:int}/GetPostById")]
        public async Task<ActionResult<PostFeedDTO>> GetPostById(int postId)
            => Ok(await service.PostService.GetPostByIdAsync(UserId, postId));


        [HttpGet("Feed")]
        public async Task<ActionResult<PagedResult<PostFeedDTO>>> GetFeed(int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetFeedAsync(UserId, page, pageSize));


        [HttpGet("{groupId:int}/GroupFeed")]
        public async Task<ActionResult<PagedResult<PostFeedDTO>>> GetGroupFeed(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetGroupFeedAsync(UserId, groupId, page, pageSize));


        [HttpPost("{postId:int}/AddLike")]
        public async Task<ActionResult<bool>> LikePost(int postId)
            => Ok(await service.PostService.LikePostAsync(postId, UserId));


        [HttpGet("{postId:int}/Likes")]
        public async Task<ActionResult<PagedResult<LikeResultDTO>>> GetLikes(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetLikesByPostIdAsync(postId, page, pageSize));


        [HttpPost("{postId:int}/AddComment")]
        public async Task<ActionResult<int>> Comment(int postId, CreateCommentDTO dto)
            => Ok(await service.PostService.AddCommentAsync(postId, dto, UserId));


        [HttpGet("{postId:int}/Comments")]
        public async Task<ActionResult<PagedResult<CommentResultDTO>>> GetComments(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetCommentsByPostIdAsync(postId, page, pageSize));

    }
}
