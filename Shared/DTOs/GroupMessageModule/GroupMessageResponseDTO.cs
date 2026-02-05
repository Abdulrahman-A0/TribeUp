using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMessages
{
    public record GroupMessageResponseDTO
    {
        public long Id { get; init; }

        public int GroupId { get; init; }
        public string GroupName { get; init; }
        public string? GroupProfilePicture { get; init; }

        public string SenderUserId { get; init; }
        public string SenderName { get; init; }
        public string? SenderProfilePicture { get; init; }

        public string Content { get; init; }
        public DateTime SentAt { get; init; }
    }
}
