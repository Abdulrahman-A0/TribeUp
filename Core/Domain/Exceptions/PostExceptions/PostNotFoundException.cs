using Domain.Exceptions.Abstraction;



namespace Domain.Exceptions.PostExceptions
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(int Id) : base($"Post With Id '{Id}' Was Not Found!")
        {
            
        }
    }
}
