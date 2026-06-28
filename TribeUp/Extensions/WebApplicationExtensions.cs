using Presentation.Hubs;
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

        public static WebApplication MapHubs(this WebApplication app)
        {
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapHub<GroupChatHub>("/hubs/group-chat");
            app.MapHub<VirtualRoomHub>("/hubs/virtual-room");
            return app;
        }

        public static WebApplication UseStaticFiles(this WebApplication app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Request.Path.StartsWithSegments("/slides"))
                    {
                        ctx.Context.Response.Headers.Append(
                            "Access-Control-Allow-Origin", "*");
                    }
                }
            });
            return app;
        }
    }
}
