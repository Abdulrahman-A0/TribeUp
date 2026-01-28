using Domain.Entities.Posts;
using Service.Specifications;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    public class PostFeedSpecification : BaseSpecifications<Post, int>
    {
        public PostFeedSpecification(
            string currentUserId,
            IQueryable<AIModeration> moderations,
            int page,
            int pageSize)
            : base(p =>
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