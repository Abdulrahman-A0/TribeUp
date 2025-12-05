using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class ServiceManager(Func<IAuthenticationService> _authFactory) : IServiceManager
    {
        public IAuthenticationService AuthenticationService => _authFactory.Invoke();
    }
}
