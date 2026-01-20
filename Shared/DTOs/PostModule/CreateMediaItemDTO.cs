using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Posts
{
    public class CreateMediaItemDTO
    {
        public string MediaURL { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
    }
}
