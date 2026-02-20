using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;

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


        [HttpPut("{postId:int}/EditPost")]
        public async Task<ActionResult<CreateEntityResultDTO>> EditPost(
            int postId,
            [FromForm] CreatePostDTO dto,
            [FromForm] List<IFormFile> mediaFiles)
            => Ok(await service.PostService.EditPostAsync(UserId, UserName, postId, dto, mediaFiles));


        [HttpDelete("{postId:int}/DeletePost")]
        public async Task<ActionResult<DeleteEntityResultDTO>> DeletePost(int postId)
            => Ok(await service.PostService.DeletePostAsync(UserId, postId));


        [HttpGet("{postId:int}/GetPostById")]
        public async Task<ActionResult<PostDTO>> GetPostById(int postId)
            => Ok(await service.PostService.GetPostByIdAsync(UserId, postId));


        [HttpGet("PersonalFeed")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetPersonalFeed(string username, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetPersonalFeedAsync(UserId, username, page, pageSize));


        [HttpGet("Feed")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetFeed(int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetFeedAsync(UserId, page, pageSize));


        [HttpGet("{groupId:int}/GroupFeed")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetGroupFeed(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetGroupFeedAsync(UserId, groupId, page, pageSize));


        [HttpPost("{postId:int}/PostToggleLike")]
        public async Task<ActionResult<ToggleLikeDTO>> ToggleLikePost(int postId)
            => Ok(await service.PostService.ToggeleLikePostAsync(UserId, UserName, postId));


        [HttpGet("{postId:int}/Likes")]
        public async Task<ActionResult<PagedResult<LikesResultDTO>>> GetLikes(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetLikesByPostIdAsync(postId, page, pageSize));


        [HttpGet("{groupId:int}/DeniedPostsByGroupId")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetDeniedPostsByGroupId(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetDeniedPostsByGroupIdAsync(UserId, groupId, page, pageSize));


        [HttpPut("ChangeEntityContentStatus")]
        public async Task<ActionResult<CreateEntityResultDTO>> ChangeEntityContentStatus(int groupId, ModerationDTO dto)
            => Ok(await service.PostService.ChangeModerationStatusAsync(UserId, groupId, dto));
    }
}
