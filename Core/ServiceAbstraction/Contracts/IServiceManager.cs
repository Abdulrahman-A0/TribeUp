using System.Reflection.Metadata.Ecma335;

namespace ServiceAbstraction.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IGroupService GroupService { get; }
        IGroupMemberService GroupMemberService { get; }
        IGroupJoinRequestService GroupJoinRequestService { get; }
        IPostService PostService { get; }
        IProfileService ProfileService { get; }
        INotificationService NotificationService { get; }
    }
}
