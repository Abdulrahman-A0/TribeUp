using Domain.Entities.Posts;
using Service.Specifications;

namespace Service.Specifications.PostSpecifications
{
    public class LikeByPostAndUserSpecification : BaseSpecifications<Like, int>
    {
        public LikeByPostAndUserSpecification(string userId, int postId)
            : base(l => l.PostId == postId && l.UserId == userId)
        {
        }
    }
}

