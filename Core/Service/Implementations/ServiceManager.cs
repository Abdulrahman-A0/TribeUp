using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class ServiceManager
        (Func<IAuthenticationService> authFactory,
         Func<IGroupService> groupServiceFactory,
         Func<IGroupMemberService> groupMemberFactory,
         Func<IGroupJoinRequestService> groupJoinRequestFactory) : IServiceManager
    {
        public IAuthenticationService AuthenticationService => authFactory.Invoke();
        public IGroupService GroupService => groupServiceFactory.Invoke();
        public IGroupMemberService GroupMemberService => groupMemberFactory.Invoke();
        public IGroupJoinRequestService GroupJoinRequestService => groupJoinRequestFactory.Invoke();
    }
}
