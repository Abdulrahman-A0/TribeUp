namespace Shared.ErrorModels
{
    public class ValidationErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message = string.Empty;
        public IEnumerable<ValidationError> Errors { get; set; } = [];
    }
}
