using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PostModule
{
    public class ModerationDTO
    {
        public ModeratedEntityType EntityType { get; set; }
        public int EntityId { get; set; }
        public ContentStatus ContentStatus { get; set; }
    }
}
