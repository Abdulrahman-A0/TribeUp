using Domain.Entities.Groups;
using Domain.Entities.Users;
using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class Event : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Upcoming;
        public string CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #region Relations

        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public virtual ICollection<EventParticipant> Participants { get; set; }
            = new HashSet<EventParticipant>();

        #endregion
    }
}