using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.ValidationExceptions
{
    public sealed class StoryContentValidationException : ValidationException
    {
        public StoryContentValidationException()
            : base(new Dictionary<string, string[]>
            {
                ["StoryContent"] = new[] { "Story must contain text or media." }
            })
        {
        }
    }
}
