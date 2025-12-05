using System.Runtime.CompilerServices;
using TribeUp.Middlewares;

namespace TribeUp.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseExceptionHandlingMiddleware(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            return app;
        }

        public static WebApplication UseSwaggerMiddleware(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
