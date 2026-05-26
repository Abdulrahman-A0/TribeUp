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

        /// <summary>
        /// Search for users by username for the tagging feature.
        /// </summary>
        /// <remarks>
        /// This endpoint is used during post/comment creation when the user types '@'.
        /// It returns matching usernames to display them in an autocomplete dropdown.
        ///
        /// Example:
        /// GET /api/users/search?username=ab
        /// </remarks>
        /// <param name="username">
        /// The username or partial username entered after '@'.
        /// </param>
        /// <returns>
        /// Returns a list of matching users including:
        /// - UserId
        /// - Username
        /// - ProfilePicture
        /// </returns>
        /// <response code="200">
        /// Users fetched successfully.
        /// </response>
        /// <response code="400">
        /// Invalid username query.
        /// </response>
        
        [HttpGet("SearchUsersByUsername")]
        public async Task<ActionResult<UsersByUsernameDTO>> SearchUsers(
            string username)
            => Ok(await service.PostService.SearchUserByUsernameAsync(username));


        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <remarks>
        /// Allows the post owner to:
        /// 
        /// - Update post content
        /// - Upload new media files
        /// - Delete existing media files
        /// 
        /// Request must be sent as multipart/form-data.
        /// 
        /// Existing media not included in deleteMediaIds remain unchanged.
        /// - param postId The ID of the target post.
        /// - param dto Updated post information.
        /// - param newMediaFiles New media files to upload.
        /// - param deleteMediaIds IDs of media files to delete. (insert the order of the media to delete into deletedMediaIds)
        /// </remarks>
        /// <returns>Returns success result containing updated post info.</returns>
        /// <response code="200">Post updated successfully.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">User is not allowed to edit this post.</response>
        /// <response code="404">Post not found.</response>

        [HttpPut("{postId:int}/EditPost")]
        public async Task<ActionResult<CreateEntityResultDTO>> EditPost(
            int postId,
            [FromForm] CreatePostDTO dto,
            [FromForm] List<IFormFile> newMediaFiles,
            [FromForm] List<int> deleteMediaIds)
            => Ok(await service.PostService.EditPostAsync(UserId, UserName, postId, dto, newMediaFiles, deleteMediaIds));


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
