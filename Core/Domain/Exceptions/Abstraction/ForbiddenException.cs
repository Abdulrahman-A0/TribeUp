namespace Domain.Exceptions.Abstraction
{
    public abstract class ForbiddenException : DomainException
    {
        protected ForbiddenException(string message) : base(message) { }
    }
}
