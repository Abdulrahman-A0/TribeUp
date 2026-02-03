using Domain.Exceptions.Abstraction;
using Shared.Enums;


namespace Domain.Exceptions.ModerationExceptions
{
    public class EntityNotFoundException : NotFoundException
    {
        public EntityNotFoundException(ModeratedEntityType entityType, int entityId)
            : base($"Entity With Type '{entityType}' & Id '{entityId}' Was Not Found!")
        {
            
        }
    }
}
