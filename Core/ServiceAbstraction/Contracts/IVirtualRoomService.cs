using Shared.DTOs.VirtualRoomModule;

namespace ServiceAbstraction.Contracts
{
    public interface IVirtualRoomService
    {
        Task<RoomDetailsDTO> JoinRoomAsync(int groupId, string userId);
        Task LeaveRoomAsync(int groupId, string userId);
        Task<IEnumerable<ParticipantDTO>> GetActiveParticipantsAsync(int groupId);
    }
}
