using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class ServiceManager(
        Func<IAuthenticationService> _authFactory,
        Func<IPostService> _postFactory
        ) : IServiceManager
    {
        public IAuthenticationService AuthenticationService => _authFactory.Invoke();

        public IPostService PostService => _postFactory.Invoke();
    }
}
