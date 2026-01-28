using Domain.Exceptions.Abstraction;
using Shared.Enums;

namespace Domain.Exceptions.GroupJoinRequestExceptions
{
    public sealed class InvalidGroupJoinRequestException
    : ConflictException
    {
        public InvalidGroupJoinRequestException(int groupId, string accessibility)
            : base($"Group '{groupId}' is {accessibility}. Join requests are only allowed for private groups.")
        {
        }
    }

}
