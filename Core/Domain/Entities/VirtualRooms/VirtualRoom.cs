using Domain.Entities.Groups;

namespace Domain.Entities.VirtualRooms
{
    public class VirtualRoom : BaseEntity<int>
    {
        public int GroupId { get; set; }
        public bool IsActive { get; set; } = false;

        // Navigation Properties
        public virtual Group Group { get; set; }
        public virtual ICollection<RoomParticipant> Participants { get; set; } = new List<RoomParticipant>();
    }
}