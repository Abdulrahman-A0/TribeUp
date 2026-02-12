using Domain.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities.Groups
{
    public class GroupFollowers : BaseEntity<int>
    {
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;

        #region Navigation properties
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
