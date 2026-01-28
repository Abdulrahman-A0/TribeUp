using Domain.Exceptions.Abstraction;

namespace Domain.Exceptions.AuthExceptions
{
    public sealed class AuthenticationFailedException : UnauthorizedDomainException
    {
        public AuthenticationFailedException
            (string errorCode = "auth_failed") : base("Authentication failed", errorCode)
        {
        }
    }
}
