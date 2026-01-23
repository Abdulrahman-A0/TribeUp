using Domain.Entities.Posts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    public class PostFeedSpecification : BaseSpecifications<Post, int>
    {
        public string CurrentUserId { get; }

        public PostFeedSpecification(string currentUserId, int page, int pageSize)
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
            )
        {
            CurrentUserId = currentUserId;

            AddIncludes(p => p.User);
            AddIncludes(p => p.Group);
            AddIncludes(p => p.Group.GroupMembers);
            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);
            AddIncludes(p => p.MediaItems);

            ApplyPagination(pageSize, page);
        }
    }
}
