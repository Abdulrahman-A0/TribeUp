using System.Reflection.Metadata.Ecma335;

namespace ServiceAbstraction.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }

        IPostService PostService { get; }
    }
}
