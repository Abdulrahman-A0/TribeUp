
namespace Domain.Contracts
{
    public static class LeaderboardBadges
    {
        public static readonly Dictionary<int, string> Badges =
            new()
            {
            {1, "Legend"},
            {2, "Master"},
            {3, "Elite"}
            };
    }
}
