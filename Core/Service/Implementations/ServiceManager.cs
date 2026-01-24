using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class ServiceManager
        (Func<IAuthenticationService> authFactory,
         Func<IGroupService> groupServiceFactory,
         Func<IGroupMemberService> groupMemberFactory,
         Func<IGroupJoinRequestService> groupJoinRequestFactory,
         Func<IPostService> _postFactory,
         Func<IProfileService> profileFactory,
         Func<INotificationService> notificationFactory) : IServiceManager
    {
        public IAuthenticationService AuthenticationService => authFactory.Invoke();
        public IGroupService GroupService => groupServiceFactory.Invoke();
        public IGroupMemberService GroupMemberService => groupMemberFactory.Invoke();
        public IGroupJoinRequestService GroupJoinRequestService => groupJoinRequestFactory.Invoke();
        public IPostService PostService => _postFactory.Invoke();
        public IProfileService ProfileService => profileFactory.Invoke();
        public INotificationService NotificationService => notificationFactory.Invoke();
    }
}
