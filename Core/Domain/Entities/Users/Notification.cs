using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Users
{
    public class Notification : BaseEntity<int>
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public int? ReferenceId { get; set; }

        #region Relations
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
