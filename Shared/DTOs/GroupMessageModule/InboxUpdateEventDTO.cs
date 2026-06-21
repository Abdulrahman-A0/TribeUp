using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMessageModule
{
    public record InboxUpdateEventDTO
    {
        public int GroupId { get; init; }
        public string LastMessage { get; init; } = string.Empty;
        public string LastMessageSenderName { get; init; } = string.Empty;
        public DateTime SentAt { get; init; }
    }
}
