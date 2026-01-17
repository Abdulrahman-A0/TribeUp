using Domain.Contracts;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.AI;
using Persistence.Data.Contexts;
using Persistence.Repositories;
using ServiceAbstraction.Contracts;
using Shared.Common;
using System.Text;

namespace TribeUp.Extensions
{
    public static class InfrastructureServicesExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("constr"));
            });

            services.AddHttpClient<IContentModerationService, PerspectiveModerationService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;


                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.ValidateJwt(configuration);

            return services;
        }
        public static IServiceCollection ValidateJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JWT").Get<JwtOptions>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });
            services.AddAuthentication();
            return services;
        }
    }
}
