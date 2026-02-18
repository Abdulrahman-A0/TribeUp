using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.PostModule;
using Shared.DTOs.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ServiceAbstraction.Contracts
{

    public interface IPostService
    {
        Task<CreateEntityResultDTO> CreatePostAsync(string userId, string username, CreatePostDTO dto, List<IFormFile> mediaFiles);
        Task<CreateEntityResultDTO> UpdatePostAsync(string userId, string username, int postId, CreatePostDTO dto, List<IFormFile> mediaFiles);
        Task<DeleteEntityResultDTO> DeletePostAsync(string userId, int postId);
        Task<PostDTO> GetPostByIdAsync(string userId, int postId);
        Task<PagedResult<PostDTO>> GetPersonalFeedAsync(string userId, int page, int pageSize);
        Task<PagedResult<PostDTO>> GetFeedAsync(string userId, int page, int pageSize);
        Task<PagedResult<PostDTO>> GetGroupFeedAsync(string userId, int groupId, int page, int pageSize);
        Task<ToggleLikeDTO> ToggeleLikePostAsync(string userId, string username, int postId);
        Task<PagedResult<LikesResultDTO>> GetLikesByPostIdAsync(int postId, int page, int pageSize);
        Task<PagedResult<PostDTO>> GetDeniedPostsByGroupIdAsync(string userId, int groupId, int page, int pageSize);
        Task<CreateEntityResultDTO> ChangeModerationStatusAsync(string userId, int groupId, ModerationDTO dto);
    }
}
