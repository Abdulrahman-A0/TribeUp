namespace Shared.DTOs.GroupInvitationModule
{
    public record InvitationDetailsDTO
    {
        public int GroupId { get; init; }
        public string GroupName { get; init; }
        public string GroupPicture { get; init; }
        public int MembersCount { get; set; }
    }
}
