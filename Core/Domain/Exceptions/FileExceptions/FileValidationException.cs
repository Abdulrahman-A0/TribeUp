using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.FileExceptions
{
    public sealed class FileValidationException : ValidationException
    {
        public FileValidationException(IDictionary<string, string[]> errors)
            : base(errors)
        {
        }
    }
}
