using ServiceAbstraction.Contracts;

namespace Service.Implementations
{
    public class ServiceManager
        (Func<IAuthenticationService> authFactory,
         Func<IGroupService> groupServiceFactory,
         Func<IGroupMemberService> groupMemberFactory,
         Func<IGroupChatService> groupChatFactory,
         Func<IPostService> _postFactory,
         Func<ICommentService> _commentFactory,
         Func<IProfileService> profileFactory,
         Func<IGroupInvitationService> invitationFactory,
         Func<INotificationService> notificationFactory) : IServiceManager
    {
        public IAuthenticationService AuthenticationService => authFactory.Invoke();
        public IGroupService GroupService => groupServiceFactory.Invoke();
        public IGroupMemberService GroupMemberService => groupMemberFactory.Invoke();
        public IGroupChatService GroupChatService => groupChatFactory.Invoke();
        public IPostService PostService => _postFactory.Invoke();
        public ICommentService CommentService => _commentFactory.Invoke();
        public IProfileService ProfileService => profileFactory.Invoke();
        public INotificationService NotificationService => notificationFactory.Invoke();
        public IGroupInvitationService GroupInvitationService => invitationFactory.Invoke();

    }
}
