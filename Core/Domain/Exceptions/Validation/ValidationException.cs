namespace Domain.Exceptions.Validation
{
    public sealed class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; set; } = [];

        public ValidationException(IEnumerable<string> errors) : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
