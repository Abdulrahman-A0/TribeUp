using Domain.Entities.Posts;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    public class GroupPostFeedSpecification : BaseSpecifications<Post, int>
    {
        public string CurrentUserId { get; }

        public GroupPostFeedSpecification(string currentUserId, int groupId, int page, int pageSize)
            : base(p =>
                    p.GroupId == groupId
                  &&
                (
                    //p.Group.GroupMembers.Any(m => m.UserId == currentUserId) ||
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