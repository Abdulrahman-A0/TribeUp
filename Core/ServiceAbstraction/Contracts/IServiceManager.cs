namespace ServiceAbstraction.Contracts
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IGroupService GroupService { get; }
        IGroupMemberService GroupMemberService { get; }
        IGroupJoinRequestService GroupJoinRequestService { get; }
    }
}
