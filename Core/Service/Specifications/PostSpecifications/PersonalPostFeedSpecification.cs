using Domain.Entities.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class PersonalPostFeedSpecification : BaseSpecifications<Post, int>
    {
        public PersonalPostFeedSpecification(
            string currentUserId,
            string username,
            IQueryable<AIModeration> moderations,
            int page,
            int pageSize)
            : base(p =>
                    p.User.UserName == username
                &&
                (
                    p.Group.Accessibility == AccessibilityType.Public ||
                    p.Group.GroupMembers.Any(m => m.UserId == currentUserId) ||
                    p.UserId == currentUserId
                )
                &&
                (
                    p.Accessibility == AccessibilityType.Public ||
                    p.Group.GroupMembers.Any(m => m.UserId == currentUserId) ||
                    p.UserId == currentUserId
                )
                && (
                 p.UserId == currentUserId ||
                    !moderations.Any(m =>
                        m.EntityType == ModeratedEntityType.Post &&
                        m.EntityId == p.Id &&
                        m.Status == ContentStatus.Denied
                    )
                )
            )
        {
            AddIncludes(p => p.User);
            AddIncludes(p => p.Tags);
            AddIncludes(p => p.Group);
            AddIncludes(p => p.Group.GroupMembers);
            AddIncludes(p => p.MediaItems);
            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);

            ApplyPagination(page, pageSize);
        }
    }
}
