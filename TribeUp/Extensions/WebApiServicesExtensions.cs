using Microsoft.AspNetCore.Mvc;
using TribeUp.Factories;

namespace TribeUp.Extensions
{
    public static class WebApiServicesExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("FrontPolicy", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod()
                    .WithOrigins(configuration["URLs:FrontUrl"]);
                });
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.CustomValidationErrorResponse;
            });

            return services;
        }
    }
}
