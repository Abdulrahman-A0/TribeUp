using Shared.DTOs.GroupJoinRequestModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupMemberModule
{
    public record JoinGroupResponseDTO
    {
        public string Message { get; init; }
        public GroupJoinRequestResultDTO Request { get; init; }
        public GroupMemberResultDTO? Follower { get; init; }
    }
}
