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
        public CommentsByPostIdSpecification(int postId)
            :base(c => true)
        {
            AddIncludes(c => c.User);   
        }
    }
}
