using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PostModule
{
    public class PostDTO
    {
        public int PostId { get; set; }
        public string? Caption { get; set; }

        public string UserId { get; set; } = null!;
        public string Username { get; set; } = null!;

        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public string GroupProfilePicture { get; set; } = null!;

        public int LikesCount { get; set; }
        public int CommentCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }
        public DateTime CreatedAt { get; set; }

        public IReadOnlyCollection<MediaItemFeedDTO> Media { get; set; }
                 = new List<MediaItemFeedDTO>();

    }
}
