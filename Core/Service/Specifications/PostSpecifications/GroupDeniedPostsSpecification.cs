using Domain.Entities.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class GroupDeniedPostsSpecification : BaseSpecifications<Post, int>
    {
        public GroupDeniedPostsSpecification(
            int groupId,
            IQueryable<AIModeration> moderations,
            int page,
            int pageSize)
            : base(p => 
                    p.GroupId == groupId
                &&
                    moderations.Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied
                    )

            )
        {

            AddIncludes(p => p.User);
            AddIncludes(p => p.Tags);
            AddIncludes(p => p.Group);
            AddIncludes(p => p.Group.GroupMembers);
            AddIncludes(p => p.MediaItems);

            ApplyPagination(page, pageSize);
        }
    }
}
