using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;

namespace TribeUp.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult CustomValidationErrorResponse(ActionContext context)
        {
            var errors = context.ModelState
                .Where(error => error.Value?.Errors.Any() == true)
                .Select(error => new ValidationError
                {
                    Field = error.Key,
                    Errors = error.Value?.Errors.Select(error => error.ErrorMessage) ?? new List<string>()
                });

            var response = new ValidationErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "One or more validation error happened",
                Errors = errors
            };

            return new BadRequestObjectResult(response);
        }
    }
}
