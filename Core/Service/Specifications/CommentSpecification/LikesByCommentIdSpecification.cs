using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.CommentSpecification
{
    public class LikesByCommentIdSpecification : BaseSpecifications<Like, int>
    {
        public LikesByCommentIdSpecification(int commentId, int pageIndex, int pageSize)
            :base(l => l.CommentId == commentId)
        {
            AddIncludes(l => l.User);
            AddIncludes(l => l.Post);

            ApplyPagination(pageIndex, pageSize);
        }
    }
}
