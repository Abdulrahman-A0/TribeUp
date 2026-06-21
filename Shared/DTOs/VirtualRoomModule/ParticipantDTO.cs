namespace Shared.DTOs.VirtualRoomModule
{
    public record ParticipantDTO
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string AvatarUrl { get; init; }
    }
}
