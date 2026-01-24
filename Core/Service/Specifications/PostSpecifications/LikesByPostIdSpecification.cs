using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    public class LikesByPostIdSpecification : BaseSpecifications<Like ,int>
    {
        public LikesByPostIdSpecification(int postId, int page, int pageSize)
            :base(l => l.PostId == postId)
        {
            AddIncludes(l => l.User);

            ApplyPagination(page, pageSize);
        }
    }
}
