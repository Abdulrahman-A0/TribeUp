using Domain.Entities.VirtualRooms;

namespace Service.Specifications.VirtualRoomSpecifications
{
    public class GetParticipantSpecification : BaseSpecifications<RoomParticipant, int>
    {
        public GetParticipantSpecification(int roomId, string userId)
            : base(p => p.VirtualRoomId == roomId && p.UserId == userId)
        {
        }
    }
}
