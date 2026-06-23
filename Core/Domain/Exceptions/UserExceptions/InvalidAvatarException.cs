using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.UserExceptions
{
    public class InvalidAvatarException : ValidationException
    {
        public InvalidAvatarException()
            : base(new Dictionary<string, string[]>
            {
                { "Avatar", ["User avatar is missing or no longer valid."] }
            })
        {
        }
    }
}
