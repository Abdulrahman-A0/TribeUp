using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.GamificationExceptions;

public class InvalidLeaderboardRangeException
    : ValidationException
{
    public InvalidLeaderboardRangeException()
        : base(new Dictionary<string, string[]>
        {
            {
                nameof(InvalidLeaderboardRangeException),
                new[]
                {
                    "Top value must be between 1 and 100"
                }
            }
        })
    {
    }
}