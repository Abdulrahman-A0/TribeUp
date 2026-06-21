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
         Func<IGroupFollowerService> groupFollowerFactory,
         Func<IStoryService> storyFactory,
         Func<ILeaderboardService> leaderboardFactory,
         Func<INotificationService> notificationFactory,
         Func<IEventService> eventFactory,
         Func<IPollService> pollFactory,
         Func<IVirtualRoomService> virtualRoomFactory) : IServiceManager
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
        public IStoryService StoryService => storyFactory.Invoke();
        public IGroupFollowerService GroupFollowerService => groupFollowerFactory.Invoke();
        public ILeaderboardService LeaderboardService => leaderboardFactory.Invoke();
        public IEventService EventService => eventFactory.Invoke();
        public IPollService PollService => pollFactory.Invoke();
        public IVirtualRoomService VirtualRoomService => virtualRoomFactory.Invoke();
    }
}
