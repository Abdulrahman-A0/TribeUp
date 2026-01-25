using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Presentation.SignalR;
using ServiceAbstraction.Contracts;
using TribeUp.BackgroundServices;
using TribeUp.Factories;
using TribeUp.Filters;

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

            services.AddSwaggerGen(swagger =>
            {

                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Eghal",
                    Description = "Eghal Blog"
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                swagger.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.CustomValidationErrorResponse;
            });

            services.AddHostedService<RefreshTokenCleanupWorker>();

            services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();

            services.AddSignalR();

            return services;
        }
    }
}
