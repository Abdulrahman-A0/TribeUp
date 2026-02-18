using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Posts
{
    public class CommentDTO
    {
        public string Content { get; set; } = null!;
        public List<string> TaggedUserIds { get; set; } = [];

    }
}
