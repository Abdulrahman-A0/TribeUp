using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Posts
{
    public class PostFeedDTO
    {
        public int PostId { get; set; }
        public string? Caption { get; set; }

        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;

        public int LikesCount { get; set; }
        public int CommentCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }

        public GroupRelationType GroupRelation { get; set; }

        public double FeedScore { get; set; }

        public DateTime CreatedAt { get; set; }

        public IReadOnlyCollection<MediaItemFeedDTO> Media { get; set; }
                 = new List<MediaItemFeedDTO>();

    }
}
