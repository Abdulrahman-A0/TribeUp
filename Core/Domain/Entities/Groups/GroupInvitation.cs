using Domain.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Groups
{
    public class GroupInvitation : BaseEntity<int>
    {
        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;
        [Required]
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsRevoked { get; set; } = false;
    }
}
