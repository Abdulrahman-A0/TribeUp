using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.PostSpecifications
{
    internal class CommentByIdSpecification : BaseSpecifications<Comment, int>
    {
        public CommentByIdSpecification(int commentId)
                    : base(c => c.Id == commentId)
        {
            AddIncludes(c => c.User);
            AddIncludes(c => c.Post);

        }
    }
}
