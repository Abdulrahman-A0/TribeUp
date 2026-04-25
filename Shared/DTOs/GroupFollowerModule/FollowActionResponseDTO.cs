
using Shared.Enums;

namespace Shared.DTOs.GroupFollowerModule
{
    public record FollowActionResponseDTO
    {
        public string Message { get; init; }
        public GroupRelationType CurrentRelation { get; init; }
    }
}
