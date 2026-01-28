using Domain.Entities.Posts;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    public class GroupPostFeedSpecification : BaseSpecifications<Post, int>
    {
        
        public GroupPostFeedSpecification(
            string currentUserId, 
            int groupId, 
            IQueryable<AIModeration> moderations, 
            int page, 
            int pageSize)
            : base(p =>
                    p.GroupId == groupId
                &&
                (
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
            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);
            AddIncludes(p => p.MediaItems);

            ApplyPagination(page, pageSize);
        }
    }
}