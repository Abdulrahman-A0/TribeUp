using Domain.Entities.Stories;
using Shared.Enums;

namespace Service.Specifications.StorySpecifications
{
    public class ActiveGroupStoriesSpecification : BaseSpecifications<Story, int>
    {
        public ActiveGroupStoriesSpecification(int groupId, bool isMember)
            : base(s => s.GroupId == groupId &&
                   s.ExpiresAt > DateTime.UtcNow &&
                   (isMember || s.Accessibility == AccessibilityType.Public))
        {
            AddIncludes(s => s.User);
            AddIncludes(s => s.StoryViews);
            AddOrderByDescending(s => s.CreatedAt);
        }
    }
}