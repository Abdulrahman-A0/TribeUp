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
        public string Caption { get; set; }
        public string MediaUrl { get; set; }
        public string GroupName { get; set; }
        public int LikesCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
