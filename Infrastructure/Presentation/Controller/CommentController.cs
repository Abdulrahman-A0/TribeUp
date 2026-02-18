using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class CommentController(IServiceManager service): ApiController
    {
        [HttpPost("{commentId:int}/CommentToggleLike")]
        public async Task<ActionResult<ToggleLikeDTO>> CommentToggleLike(int commentId)
            => Ok(await service.CommentService.CommentToggleLikeAsync(UserId, UserName, commentId));


        [HttpGet("{commentId:int}/Likes")]
        public async Task<ActionResult<PagedResult<LikesResultDTO>>> GetLikes(int commentId, int page = 1, int pageSize = 20)
            => Ok(await service.CommentService.GetLikesByCommentIdAsync(commentId, page, pageSize));

        [HttpPost("{postId:int}/AddComment")]
        public async Task<ActionResult<CreateEntityResultDTO>> Comment(int postId, CommentDTO dto)
            => Ok(await service.CommentService.AddCommentAsync(UserId, UserName, postId, dto));


        [HttpPut("{commentId:int}/UpdateComment")]
        public async Task<ActionResult<CreateEntityResultDTO>> UpdateComment(int commentId, CommentDTO dto)
            => Ok(await service.CommentService.UpdateCommentAsync(UserId, UserName, commentId, dto));


        [HttpDelete("{commentId:int}/DeleteComment")]
        public async Task<ActionResult<DeleteEntityResultDTO>> DeleteComment(int commentId)
           => Ok(await service.CommentService.DeleteCommentAsync(UserId, commentId));


        [HttpGet("{postId:int}/Comments")]
        public async Task<ActionResult<PagedResult<CommentResultDTO>>> GetComments(int postId, int page = 1, int pageSize = 20)
            => Ok(await service.CommentService.GetCommentsByPostIdAsync(UserId, postId, page, pageSize));

    }
}
