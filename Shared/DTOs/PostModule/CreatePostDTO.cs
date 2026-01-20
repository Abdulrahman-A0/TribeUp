using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Posts
{
    public class CreatePostDTO
    {
        public int GroupId { get; set; }
        public string? Caption { get; set; }
        public List<CreateMediaItemDTO>? MediaItems { get; set; }
    }
}
