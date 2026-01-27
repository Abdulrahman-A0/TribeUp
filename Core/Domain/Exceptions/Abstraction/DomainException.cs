namespace Domain.Exceptions.Abstraction
{
    public abstract class DomainException : Exception
    {
        public string? ErrorCode { get; }
        protected DomainException(string message, string? errorCode = null)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }


}
