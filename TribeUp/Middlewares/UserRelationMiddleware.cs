using System.Security.Claims;
using ServiceAbstraction.Contracts;

namespace TribeUp.Middlewares
{

    public class UserRelationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserRelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserGroupRelationService relationService)
        {
            var userId = context.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await relationService.InitializeAsync(userId);
            }

            await _next(context);
        }
    }

}
