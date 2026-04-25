using Domain.Entities.Stories;
using Service.Specifications;
using System;

namespace Service.Specifications.StorySpecifications
{
    public class StoryWithViewsByIdSpecification : BaseSpecifications<Story, int>
    {
        public StoryWithViewsByIdSpecification(int storyId)
            : base(s => s.Id == storyId &&
                  (s.ExpiresAt == null || s.ExpiresAt > DateTime.UtcNow))
        {
            AddIncludes(s => s.User);
            AddIncludes(s => s.StoryViews);
        }
    }
}