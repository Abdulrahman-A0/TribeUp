using Domain.Entities.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class CommentsByPostIdSpecification : BaseSpecifications<Comment, int>
    {
        public CommentsByPostIdSpecification(
            string currentUserId,
            IQueryable<AIModeration> moderations,
            int postId,
            int page,
            int pageSize)
            :base(c => 
                    c.PostId == postId

            &&
                (
                c.UserId == currentUserId ||
                    !moderations.Any(m =>
                        m.EntityType == ModeratedEntityType.Comment &&
                        m.EntityId == c.Id &&
                        m.Status == ContentStatus.Denied
                    )
               )
            )
        {
            AddIncludes(c => c.User);

            ApplyPagination(page, pageSize);
        }
    }
}
