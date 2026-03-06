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
            int groupId,
            string postAuthorId,
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
            AddIncludes(c => c.Likes);

            AddIncludes(c => c.Post.Group.GroupMembers);
            AddIncludes(c => c.Post.Group.GroupFollowers);

            AddOrderByDescending(c =>

                c.UserId == postAuthorId ? 120 :
            
                c.Post.Group.GroupMembers
                    .Where(m => m.UserId == c.UserId)
                    .Select(m => m.Role)
                    .FirstOrDefault() == RoleType.Owner ? 90 :
            
                c.Post.Group.GroupMembers
                    .Where(m => m.UserId == c.UserId)
                    .Select(m => m.Role)
                    .FirstOrDefault() == RoleType.Admin ? 80 :
            
                c.Post.Group.GroupMembers
                    .Where(m => m.UserId == c.UserId)
                    .Select(m => m.Role)
                    .FirstOrDefault() == RoleType.Member ? 70 :
            
                c.Post.Group.GroupFollowers
                    .Any(f => f.UserId == c.UserId) ? 50 :
            
                10
            );

            AddOrderByDescending(c => c.Likes.Count);
            AddOrderByDescending(c => c.CreatedAt);


            ApplyPagination(page, pageSize);
        }
    }
}
