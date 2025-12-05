namespace ServiceAbstraction.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
    }
}
