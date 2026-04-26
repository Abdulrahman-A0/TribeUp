using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMemberModule
{
    public record GroupMemberResultDTO
    {
        public int Id { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string UserProfilePicture { get; init; } = string.Empty;
        public string Role { get; init; }
        public DateTime JoinedAt { get; init; }
    }
}
