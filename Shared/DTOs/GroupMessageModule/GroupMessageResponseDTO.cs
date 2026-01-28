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
        public string UserId { get; init; }
        public string UserName { get; init; }
        public string Content { get; init; }
        public DateTime SentAt { get; init; }
    }
}
