namespace Domain.Exceptions.Abstraction
{
    public abstract class UnauthorizedDomainException : DomainException
    {
        protected UnauthorizedDomainException(
            string message = "Unauthorized",
            string? errorCode = null)
            : base(message, errorCode)
        {

        }
    }


}
