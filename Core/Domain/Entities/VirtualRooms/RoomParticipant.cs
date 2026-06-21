using Domain.Entities.Users;

namespace Domain.Entities.VirtualRooms
{
    public class RoomParticipant : BaseEntity<int>
    {
        public int VirtualRoomId { get; set; }
        public string UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual VirtualRoom VirtualRoom { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}