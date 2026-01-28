using Presentation.SignalR;
using Service;
using Service.Implementations;
using ServiceAbstraction.Contracts;
using Shared.Common;

namespace TribeUp.Extensions
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { }, typeof(AssemblyReference).Assembly);

            services.Configure<JwtOptions>(configuration.GetSection("JWT"));
            services.Configure<EmailOptions>(configuration.GetSection("Email"));
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddScoped<IServiceManager, ServiceManager>();


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IMediaUrlService, MediaUrlService>();

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IGroupJoinRequestService, GroupJoinRequestService>();
            services.AddScoped<IGroupScoreService, GroupScoreService>();
            services.AddScoped<IGroupChatService, GroupChatService>();
            services.AddScoped<IGroupChatPermissionService, GroupChatPermissionService>();


            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostUrlService, PostUrlService>();
            services.AddScoped<IAIModerationManager, AIModerationManager>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<INotificationService, NotificationService>();



            services.AddScoped<Func<IAuthenticationService>>(provider =>
            () => provider.GetRequiredService<IAuthenticationService>());

            services.AddScoped<Func<IGroupService>>(provider =>
            () => provider.GetRequiredService<IGroupService>());

            services.AddScoped<Func<IGroupMemberService>>(provider =>
            () => provider.GetRequiredService<IGroupMemberService>());

            services.AddScoped<Func<IGroupJoinRequestService>>(provider =>
            () => provider.GetRequiredService<IGroupJoinRequestService>());

            services.AddScoped<Func<IGroupScoreService>>(provider =>
            () => provider.GetRequiredService<IGroupScoreService>());
            
            services.AddScoped<Func<IGroupChatService>>(provider =>
            () => provider.GetRequiredService<IGroupChatService>());

            services.AddScoped<Func<IGroupChatPermissionService>>(provider =>
            () => provider.GetRequiredService<IGroupChatPermissionService>());


            services.AddScoped<Func<IPostService>>(provider =>
                () => provider.GetRequiredService<IPostService>()
            );
            services.AddScoped<Func<IProfileService>>(provider =>
            () => provider.GetRequiredService<IProfileService>()
            );

            services.AddScoped<Func<INotificationService>>(provider =>
            () => provider.GetRequiredService<INotificationService>());


            return services;
        }
    }
}
