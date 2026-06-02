using Domain.Entities.Users;
using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class EventParticipant : BaseEntity<int>
    {
        public int EventId { get; set; }

        public string UserId { get; set; }

        public EventResponseStatus Status { get; set; }

        public DateTime RespondedAt { get; set; } = DateTime.UtcNow;

        #region Relations

        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}