using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.StoryExceptions
{
    public class StoryNotFoundException : NotFoundException
    {
        public StoryNotFoundException(int storyId)
            : base($"Story with id: {storyId} not found")
        {
        }
    }
}