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
            services.AddScoped<IServiceManager, ServiceManager>();


            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IGroupJoinRequestService, GroupJoinRequestService>();
            services.AddScoped<IPostService, PostService>();



            services.AddScoped<Func<IAuthenticationService>>(provider =>
            () => provider.GetRequiredService<IAuthenticationService>());

            services.AddScoped<Func<IGroupService>>(provider =>
            () => provider.GetRequiredService<IGroupService>());

            services.AddScoped<Func<IGroupMemberService>>(provider =>
            () => provider.GetRequiredService<IGroupMemberService>());

            services.AddScoped<Func<IGroupJoinRequestService>>(provider =>
            () => provider.GetRequiredService<IGroupJoinRequestService>());
            () => provider.GetRequiredService<IAuthenticationService>()
            );

            services.AddScoped<Func<IPostService>>(provider =>
                () => provider.GetRequiredService<IPostService>()
            );


            services.Configure<JwtOptions>(configuration.GetSection("JWT"));

            return services;
        }
    }
}
