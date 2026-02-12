using Domain.Entities.Users;
using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Groups
{
    public class GroupMembers : BaseEntity<int>
    {
        public RoleType Role { get; set; } = RoleType.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;


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
