namespace Domain.Exceptions.Abstraction
{
    public abstract class ConflictException : DomainException
    {
        protected ConflictException(string message) : base(message) { }
    }


}
