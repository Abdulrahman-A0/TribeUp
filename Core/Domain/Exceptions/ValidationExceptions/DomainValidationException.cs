using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.ValidationExceptions
{
    public sealed class DomainValidationException : ValidationException
    {
        public DomainValidationException(
            IDictionary<string, string[]> errors)
            : base(errors)
        {
        }
    }

}
