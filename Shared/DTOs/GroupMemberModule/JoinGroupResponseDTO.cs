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
        public bool IsImmediate { get; init; }
        public string Message { get; init; }
        public GroupMemberResultDTO? MemberResult { get; init; }
        public GroupJoinRequestResultDTO? RequestResult { get; init; }
    }
}
