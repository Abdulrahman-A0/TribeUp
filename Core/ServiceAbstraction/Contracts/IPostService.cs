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
        Task<IEnumerable<FeedPostDTO>> GetFeedAsync();
        Task LikePostAsync(int postId, string userId);
        Task AddCommentAsync(int postId, CreateCommentDTO dto, string userId);
    }
}
