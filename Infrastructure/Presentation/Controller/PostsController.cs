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

        /// <summary>
        /// Retrieves all denied posts for a specific group.
        /// </summary>
        /// <remarks>
        /// This endpoint returns posts that were rejected by the AI moderation system and are currently marked as Denied.
        /// Only Group Admins and Owners can access this endpoint.
        ///
        /// Pagination is supported through the <c>page</c> and <c>pageSize</c> parameters.
        ///
        /// Parameters:
        /// - groupId: The identifier of the group whose denied posts should be retrieved.
        /// - page: The page number to retrieve. Must be greater than 0.
        /// - pageSize: Number of posts per page. Must be greater than 0.
        ///
        /// Returned posts include the standard post information with <c>IsDenied = true</c>.
        ///
        /// Authorization:
        /// - Owner
        /// - Admin
        ///
        /// Possible Errors:
        /// - 403 Forbidden: User is not an Admin or Owner of the group.
        /// - 404 Not Found: Group does not exist.
        /// - 400 Bad Request: Invalid page or pageSize value.
        /// </remarks>
        /// <param name="groupId">The target group identifier.</param>
        /// <param name="page">Page number (starting from 1).</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>A paginated list of denied posts.</returns>
        
        [HttpGet("{groupId:int}/DeniedPostsByGroupId")]
        public async Task<ActionResult<PagedResult<PostDTO>>> GetDeniedPostsByGroupId(int groupId, int page = 1, int pageSize = 20)
            => Ok(await service.PostService.GetDeniedPostsByGroupIdAsync(UserId, groupId, page, pageSize));

        /// <summary>
        /// Updates the moderation status of a post or comment.
        /// </summary>
        /// <remarks>
        /// This endpoint allows Group Admins and Owners to manually review content that was previously flagged by the AI moderation system.
        ///
        /// Depending on the selected status:
        ///
        /// - Accepted:
        ///     - The content is approved.
        ///     - Group score is increased.
        ///     - Returns "Accepted by Admins."
        ///
        /// - Denied:
        ///     - The content remains rejected.
        ///     - Group score is decreased.
        ///     - Returns "Denied by Admins."
        ///
        /// Supported Entity Types:
        /// - 0 = Post
        /// - 1 = Comment
        ///
        /// Supported Content Status Values:
        /// - 0 = Accepted
        /// - 1 = Denied
        ///
        /// Request Body Example:
        ///
        /// {
        ///   "entityType": 0,
        ///   "entityId": 15,
        ///   "contentStatus": 0
        /// }
        ///
        /// In this example:
        /// - entityType = Post
        /// - entityId = 15
        /// - contentStatus = Accepted
        ///
        /// Authorization:
        /// - Owner
        /// - Admin
        ///
        /// Possible Errors:
        /// - 403 Forbidden: User is not an Admin or Owner.
        /// - 404 Not Found: Group does not exist.
        /// - 404 Not Found: Moderation record was not found for the specified entity.
        /// </remarks>
        /// <param name="groupId">The group that owns the moderated content.</param>
        /// <param name="dto">
        /// Moderation request containing:
        /// - EntityType (0 = Post, 1 = Comment)
        /// - EntityId (Target post or comment identifier)
        /// - ContentStatus (0 = Accepted, 1 = Denied)
        /// </param>
        /// <returns>The moderation result and updated status.</returns>

        [HttpPut("ChangeEntityContentStatus")]
        public async Task<ActionResult<CreateEntityResultDTO>> ChangeEntityContentStatus(int groupId, ModerationDTO dto)
            => Ok(await service.PostService.ChangeModerationStatusAsync(UserId, groupId, dto));
    }
}
