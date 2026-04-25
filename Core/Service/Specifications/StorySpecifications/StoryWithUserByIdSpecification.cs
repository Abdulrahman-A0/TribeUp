using Domain.Entities.Stories;

namespace Service.Specifications.StorySpecifications
{
    public class StoryWithUserByIdSpecification : BaseSpecifications<Story, int>
    {
        public StoryWithUserByIdSpecification(int storyId)
            : base(s => s.Id == storyId)
        {
            AddIncludes(s => s.User);
            AddIncludes(s => s.Group);
        }
    }
}
