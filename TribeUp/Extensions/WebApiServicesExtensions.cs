using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Presentation.SignalR;
using ServiceAbstraction.Contracts;
using Shared.ErrorModels;
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
                    Title = "TribeUp",
                    Description = "TribeUp Social Media"
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
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors.Any() == true)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    var problem = new ApiProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Type = "validation_error",
                        Title = "Validation failed",
                        Errors = errors,
                        TraceId = context.HttpContext.TraceIdentifier
                    };

                    return new BadRequestObjectResult(problem);
                };
            });

            services.AddHostedService<RefreshTokenCleanupWorker>();

            services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();
            services.AddScoped<IGroupChatNotifier, SignalRGroupChatNotifier>();

            services.AddSignalR();

            return services;
        }
    }
}
