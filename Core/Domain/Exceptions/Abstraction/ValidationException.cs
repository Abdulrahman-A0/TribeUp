namespace Domain.Exceptions.Abstraction
{
    public abstract class ValidationException : DomainException
    {
        public IDictionary<string, string[]> Errors { get; }

        protected ValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }


}
