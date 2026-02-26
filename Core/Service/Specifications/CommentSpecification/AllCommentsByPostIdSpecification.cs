using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.CommentSpecification
{
    public class AllCommentsByPostIdSpecification : BaseSpecifications<Comment, int>
    {
        public AllCommentsByPostIdSpecification(int postId)
            :base(c => c.PostId == postId)
        {
            AddIncludes(c => c.Likes);
        }
    }
}
