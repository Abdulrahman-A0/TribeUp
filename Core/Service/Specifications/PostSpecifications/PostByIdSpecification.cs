using Domain.Entities.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class PostByIdSpecification : BaseSpecifications<Post, int>
    {

        public PostByIdSpecification(int postId)
            : base(p =>
                //(
                //    p.Group.Accessibility == AccessibilityType.Public ||
                //    p.Group.GroupMembers.Any(m => m.UserId == currentUserId) ||
                //    p.UserId == currentUserId
                //)
                //&&
                //(
                //    p.Accessibility == AccessibilityType.Public ||
                //    p.Group.GroupMembers.Any(m => m.UserId == currentUserId) ||
                //    p.UserId == currentUserId
                //)
                //&&
                    p.Id == postId
            )
        {


            AddIncludes(p => p.User);
            AddIncludes(p => p.Group);
            AddIncludes(p => p.Group.GroupMembers);
            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);
            AddIncludes(p => p.MediaItems);

        }
    }
}
