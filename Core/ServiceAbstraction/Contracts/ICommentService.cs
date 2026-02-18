using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface ICommentService
    {
        Task<ToggleLikeDTO> CommentToggleLikeAsync(string userId, string username, int commentId);
        public Task<PagedResult<LikesResultDTO>> GetLikesByCommentIdAsync(int commentId, int page, int pageSize);
        Task<CreateEntityResultDTO> AddCommentAsync(string userId, string username, int postId, CommentDTO dto);
        Task<DeleteEntityResultDTO> DeleteCommentAsync(string userId, int commentId);
        Task<CreateEntityResultDTO> UpdateCommentAsync(string userId, string username, int commentId, CommentDTO dto);
        Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(string userId, int postId, int page, int pageSize);

    }
}
