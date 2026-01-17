using Domain.Entities.Posts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enums;

namespace Service.Specifications.PostSpecifications
{
    internal class PostFeedSpecification : BaseSpecifications<Post,int>
    {
        public PostFeedSpecification() 
            :base(p => p.Accessibility == AccessibilityType.Public &&
                  (p.AI_Moderation == null ||
                    p.AI_Moderation.Status != ContentStatus.Denied))
        {
            AddIncludes(p => p.Group);
            AddIncludes(p => p.Likes);
            AddIncludes(p => p.Comments);
            AddIncludes(p => p.MediaItems); 
            AddIncludes(p => p.AI_Moderation);

        }
    }
}
