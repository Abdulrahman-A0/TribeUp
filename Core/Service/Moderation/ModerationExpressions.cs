using System.Linq.Expressions;
using Service.Moderation;

namespace Service.Moderation
{
    public static class ModerationExpressions
    {
        public static Expression<Func<T, bool>> VisibleToUser<T>(
            string currentUserId,
            Expression<Func<T, string>> ownerSelector,
            Expression<Func<T, bool>> isDeniedSelector)
        {
            return entity =>
                ModerationVisibilityQuery.IsVisible(
                    ownerSelector.Compile()(entity),
                    currentUserId,
                    isDeniedSelector.Compile()(entity)
                );
        }
    }
}
