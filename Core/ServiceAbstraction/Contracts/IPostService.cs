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
        Task<CreatePostResultDTO> CreatePostAsync(CreatePostDTO dto, string userId, List<IFormFile> mediaFiles);
        Task<PostDTO> GetPostByIdAsync(string userId, int postId);
        Task<PagedResult<PostFeedDTO>> GetFeedAsync(string userId, int page, int pageSize);
        Task<PagedResult<PostFeedDTO>> GetGroupFeedAsync(string userId, int groupId, int page, int pageSize);
        Task<bool> LikePostAsync(int postId, string userId);
        Task<PagedResult<LikeResultDTO>> GetLikesByPostIdAsync(int postId, int page, int pageSize);
        //Task<int> AddCommentAsync(int postId, CreateCommentDTO dto, string userId);
        //Task<PagedResult<CommentResultDTO>> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    }
}
