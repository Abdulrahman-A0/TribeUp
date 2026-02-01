using Domain.Exceptions.Abstraction;
using Shared.ErrorModels;

namespace TribeUp.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var problem = new ApiProblemDetails
            {
                TraceId = context.TraceIdentifier
            };

            switch (ex)
            {
                case ValidationException ve:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Type = ErrorTypes.Validation;
                    problem.Title = "Validation failed";
                    problem.Errors = ve.Errors;
                    break;

                case UnauthorizedDomainException auth:
                    problem.Status = StatusCodes.Status401Unauthorized;
                    problem.Type = ErrorTypes.Unauthorized;
                    problem.Title = auth.ErrorCode switch
                    {
                        "invalid_credentials" => "Invalid email or password",
                        _ => "Unauthorized"
                    };

                    problem.Extensions["code"] = auth.ErrorCode;
                    break;

                case NotFoundException notFound:
                    problem.Status = StatusCodes.Status404NotFound;
                    problem.Type = ErrorTypes.NotFound;
                    problem.Title = notFound.Message;
                    break;

                case ConflictException conflict:
                    problem.Status = StatusCodes.Status409Conflict;
                    problem.Type = ErrorTypes.Conflict;
                    problem.Title = conflict.Message;
                    break;
               
                case ForbiddenException forbbiden:
                    problem.Status = StatusCodes.Status403Forbidden;
                    problem.Type = ErrorTypes.Forbidden;
                    problem.Title = forbbiden.Message;
                    break;

                default:
                    _logger.LogError($"Something went wrong ==> : {ex.Message}");

                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Type = ErrorTypes.ServerError;
                    problem.Title = "Internal server error";
                    break;
            }

            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
