using Domain.Contracts;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Contexts;
using Persistence.Repositories;

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
            
            return services;
         }
    }
}
