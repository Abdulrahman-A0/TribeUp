using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Posts
{
    public class FeedPostDTO
    {
        public int PostId { get; set; }
        public string? Caption { get; set; }
        public string GroupName { get; set; } = null!;
        public int LikesCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public IReadOnlyCollection<MediaItemDTO> Media { get; set; } = new List<MediaItemDTO>();
    }
}
