using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.FileExceptions
{
    public sealed class FileStorageFailedException : DomainException
    {
        public FileStorageFailedException()
            : base("File storage operation failed")
        {
        }
    }
}
