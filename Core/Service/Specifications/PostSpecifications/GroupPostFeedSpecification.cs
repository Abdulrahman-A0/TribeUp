using Domain.Entities.Posts;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    public class GroupPostFeedSpecification : BaseSpecifications<Post, int>
    {
        public GroupPostFeedSpecification(string userId, int groupId)
            : base(p =>
                    p.GroupId == groupId &&

                (
                    p.AI_Moderation.Status != ContentStatus.Denied ||
                    p.UserId == userId
                ) &&

                (
                    p.Group.Accessibility == AccessibilityType.Public ||
                    p.Group.GroupMembers.Any(m => m.UserId == userId) ||
                    p.Group.GroupFollowers.Any(f => f.UserId == userId) ||
                    p.UserId == userId
                ) &&

                (
                    p.Accessibility == AccessibilityType.Public ||
                    p.Group.GroupMembers.Any(m => m.UserId == userId) ||
                    p.Group.GroupFollowers.Any(f => f.UserId == userId) ||
                    p.UserId == userId
                )
            )
        {
            AddIncludes(p => p.User);

            AddIncludes(p => p.Group);
            AddIncludes(p => p.Group.GroupMembers);
            AddIncludes(p => p.Group.GroupFollowers);

            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);
            AddIncludes(p => p.MediaItems);
            AddIncludes(p => p.AI_Moderation);
        }
    }
}
