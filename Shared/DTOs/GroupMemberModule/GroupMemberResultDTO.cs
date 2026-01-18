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
        public string UserId { get; init; }
        public string UserName { get; init; }
        public string UserProfilePicture { get; init; }
        public RoleType Role { get; init; }
        public DateTime JoinedAt { get; init; }
    }
}
