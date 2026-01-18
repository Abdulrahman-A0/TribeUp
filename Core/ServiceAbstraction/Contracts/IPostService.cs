using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs.Posts;
namespace ServiceAbstraction.Contracts
{
    public interface IPostService
    {
        Task CreatePostAsync(CreatePostDTO dto, string userId);
        Task<PagedResult<PostFeedDTO>> GetFeedAsync(string userId, int page, int pageSize);
        Task<bool> LikePostAsync(int postId, string userId);
        Task AddCommentAsync(int postId, CreateCommentDTO dto, string userId);
    }
}
