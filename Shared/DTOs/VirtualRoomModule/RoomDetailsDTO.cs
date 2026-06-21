namespace Shared.DTOs.VirtualRoomModule
{
    public record RoomDetailsDTO(
        int RoomId,
        int GroupId,
        IEnumerable<ParticipantDTO> Participants
    );
}
