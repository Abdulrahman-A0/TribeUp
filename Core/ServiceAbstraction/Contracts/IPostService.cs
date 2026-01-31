using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ServiceAbstraction.Contracts
{

    public interface IPostService
    {
        Task<CreateEntityResultDTO> CreatePostAsync(string userId, CreatePostDTO dto, List<IFormFile> mediaFiles);
        Task<CreateEntityResultDTO> UpdatePostAsync(string userId,int postId, CreatePostDTO dto, List<IFormFile> mediaFiles);
        Task<DeleteEntityResultDTO> DeletePostAsync(string userId, int postId);
        Task<PostDTO> GetPostByIdAsync(string userId, int postId);
        Task<PagedResult<PostDTO>> GetFeedAsync(string userId, int page, int pageSize);
        Task<PagedResult<PostDTO>> GetGroupFeedAsync(string userId, int groupId, int page, int pageSize);
        Task<ToggleLikeDTO> ToggeleLikePostAsync(string userId, int postId);
        Task<PagedResult<LikesResultDTO>> GetLikesByPostIdAsync(int postId, int page, int pageSize);
        Task<CreateEntityResultDTO> AddCommentAsync(string userId, int postId, CommentDTO dto);
        Task<DeleteEntityResultDTO> DeleteCommentAsync(string userId, int commentId);
        Task<CreateEntityResultDTO> UpdateCommentAsync(string userId, int commentId, CommentDTO dto);
        Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    }
}
