using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupFollowerModule
{
    public record GroupFollowerResultDTO
    {
        public string UserId { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string ProfilePictureUrl { get; init; } = string.Empty;
        public DateTime FollowedAt { get; init; } = DateTime.UtcNow;
    }
}
