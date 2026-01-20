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
        Task<CreatePostResultDTO> CreatePostAsync(CreatePostDTO dto, string userId);
        Task<PagedResult<PostFeedDTO>> GetFeedAsync(string userId, int page, int pageSize);
        Task<bool> LikePostAsync(int postId, string userId);
        Task<int> AddCommentAsync(int postId, CreateCommentDTO dto, string userId);
    }
}
