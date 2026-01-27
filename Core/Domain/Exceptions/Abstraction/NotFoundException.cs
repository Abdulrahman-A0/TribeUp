namespace Domain.Exceptions.Abstraction
{
    public abstract class NotFoundException : DomainException
    {
        protected NotFoundException(string message) : base(message) { }
    }


}
