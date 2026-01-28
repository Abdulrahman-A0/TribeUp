using Microsoft.AspNetCore.Mvc;

namespace Shared.ErrorModels
{
    public class ApiProblemDetails : ProblemDetails
    {
        public IDictionary<string, string[]>? Errors { get; set; }
        public string? TraceId { get; set; }
    }

}
