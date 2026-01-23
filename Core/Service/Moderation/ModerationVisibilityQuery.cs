using Shared.Enums;

namespace Service.Moderation
{
    public static class ModerationVisibilityQuery
    {
        public static bool IsVisible(
            string ownerUserId,
            string currentUserId,
            bool isDenied)
        {
            if (ownerUserId == currentUserId)
                return true;

            return !isDenied;
        }
    }
}
