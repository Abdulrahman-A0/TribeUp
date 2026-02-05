using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMessageModule
{
    public record GroupChatInboxDTO
    {
        public int GroupId { get; init; }
        public string GroupName { get; init; }
        public string? GroupProfilePicture { get; init; }

        public string LastMessageContent { get; init; }
        public string LastMessageSenderName { get; init; }
        public DateTime LastMessageSentAt { get; init; }
    }
}
