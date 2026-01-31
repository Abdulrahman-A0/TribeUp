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
        public async Task<ActionResult<CreateEntityResultDTO>> CreatePost(
            [FromForm] CreatePostDTO dto,
            [FromForm] List<IFormFile> mediaFiles)
            => Ok(await service.PostService.CreatePostAsync(UserId, UserName, dto, mediaFiles));


        [HttpPut("{postId:int}/UpdatePost")]
        public async Task<ActionResult<CreateEntityResultDTO>> UpdatePost(
            int postId,
            [FromForm] CreatePostDTO dto,
            [FromForm] List<IFormFile> mediaFiles)
            => Ok(await service.PostService.UpdatePostAsync(UserId, UserName, postId, dto, mediaFiles));


        [HttpDelete("{postId:int}/DeletePost")]
        public async Task<ActionResult<DeleteEntityResultDTO>> DeletePost(int postId)
            => Ok(await service.PostService.DeletePostAsync(UserId, postId));


        [HttpGet("{postId:int}/GetPostById")]
        public async Task<ActionResult<PostDTO>> GetPostById(int postId)
            => Ok(await service.PostService.GetPostByIdAsync(UserId, postId));


        [HttpGet("Feed")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetFeed(int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetFeedAsync(UserId, page, pageSize));


        [HttpGet("{groupId:int}/GroupFeed")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetGroupFeed(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetGroupFeedAsync(UserId, groupId, page, pageSize));


        [HttpPost("{postId:int}/ToggleLike")]
        public async Task<ActionResult<ToggleLikeDTO>> ToggleLikePost(int postId)
            => Ok(await service.PostService.ToggeleLikePostAsync(UserId, UserName, postId));


        [HttpGet("{postId:int}/Likes")]
        public async Task<ActionResult<PagedResult<LikesResultDTO>>> GetLikes(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetLikesByPostIdAsync(postId, page, pageSize));


        [HttpPost("{postId:int}/AddComment")]
        public async Task<ActionResult<CreateEntityResultDTO>> Comment(int postId, CommentDTO dto)
            => Ok(await service.PostService.AddCommentAsync(UserId, UserName, postId, dto));


        [HttpPut("{commentId:int}/UpdateComment")]
        public async Task<ActionResult<CreateEntityResultDTO>> UpdateComment(int commentId, CommentDTO dto)
            => Ok(await service.PostService.UpdateCommentAsync(UserId, UserName, commentId , dto));


        [HttpDelete("{commentId:int}/DeleteComment")]
        public async Task<ActionResult<DeleteEntityResultDTO>> DeleteComment(int commentId)
           => Ok(await service.PostService.DeleteCommentAsync(UserId, commentId));


        [HttpGet("{postId:int}/Comments")]
        public async Task<ActionResult<PagedResult<CommentResultDTO>>> GetComments(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetCommentsByPostIdAsync(postId, page, pageSize));

    }
}
