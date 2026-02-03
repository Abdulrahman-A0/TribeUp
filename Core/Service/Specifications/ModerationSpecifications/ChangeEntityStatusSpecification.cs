using Domain.Entities.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.ModerationSpecifications
{
    public class ChangeEntityStatusSpecification : BaseSpecifications<AIModeration, int>
    {
        public ChangeEntityStatusSpecification(ModeratedEntityType entityType, int entityId)
            :base(m =>
                m.EntityType == entityType
                &&
                m.EntityId == entityId
            )

        {
            
        }
    }
}
