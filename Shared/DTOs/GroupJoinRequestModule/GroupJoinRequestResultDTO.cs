using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GroupJoinRequestModule
{
    public record GroupJoinRequestResultDTO
    {
        public int Id { get; init; }

        public int GroupId { get; init; }
        public string GroupName { get; init; }

        public string UserId { get; init; }
        public string UserName { get; init; }

        public JoinRequestStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
