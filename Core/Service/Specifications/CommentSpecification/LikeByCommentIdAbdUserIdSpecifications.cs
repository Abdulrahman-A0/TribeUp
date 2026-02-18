using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specifications.CommentSpecification
{
    public class LikeByCommentIdAbdUserIdSpecifications : BaseSpecifications<Like,int>
    {
        public LikeByCommentIdAbdUserIdSpecifications(string userId, int commentId)
            :base(c => c.CommentId == commentId && c.UserId == userId)
        {
            
        }
    }
}
