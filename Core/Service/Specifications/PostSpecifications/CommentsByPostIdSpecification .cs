using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class CommentsByPostIdSpecification : BaseSpecifications<Comment, int>
    {
        public CommentsByPostIdSpecification(int postId, int page, int pageSize)
            :base(c => c.PostId == postId)
        {
            AddIncludes(c => c.User);

            ApplyPagination(page, pageSize);
        }
    }
}
