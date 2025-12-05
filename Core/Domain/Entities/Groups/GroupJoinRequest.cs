using Domain.Entities.Users;
using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Groups
{
    public class GroupJoinRequest : BaseEntity<int>
    {
        public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }
    }
}
