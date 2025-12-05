
using Domain.Exceptions.UnAuthorized;
using Domain.Exceptions.Validation;
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
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                    await HandleNotFoundApiAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong ==> : {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                Message = ex.Message
            };

            context.Response.StatusCode = ex switch
            {
                UnAuthorizedException => StatusCodes.Status401Unauthorized,
                ValidationException validationException => HandleValidationException(validationException, response),
                _ => StatusCodes.Status500InternalServerError
            };

            response.StatusCode = context.Response.StatusCode;
            await context.Response.WriteAsync(response.ToString());
        }


        private async Task HandleNotFoundApiAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorDetails
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"The endpoint with url: '{context.Request.Path}' not found"
            }.ToString();

            await context.Response.WriteAsync(response);
        }
        private int HandleValidationException(ValidationException validationException, ErrorDetails response)
        {
            response.Errors = validationException.Errors;
            return StatusCodes.Status400BadRequest;
        }
    }
}
