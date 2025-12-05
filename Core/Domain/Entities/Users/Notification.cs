using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Users
{
    public class Notification : BaseEntity<int>
    {
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        #region Relations
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
