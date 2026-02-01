using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.ForbiddenExceptions
{
    public class ForbiddenActionException : ForbiddenException
    {
        public ForbiddenActionException() : base("You do not have permission to perform this action.")
        {
            
        }
    }
}
